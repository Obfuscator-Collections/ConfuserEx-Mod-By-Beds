using System;
using System.Collections.Generic;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace KoiVM
{
	public class GenericInstantiation
	{
		private readonly Dictionary<MethodSpec, MethodDef> instantiations = new Dictionary<MethodSpec, MethodDef>(MethodEqualityComparer.CompareDeclaringTypes);

		public event Func<MethodSpec, bool> ShouldInstantiate;

		public void EnsureInstantiation(MethodDef method, Action<MethodSpec, MethodDef> onInstantiated)
		{
			foreach (Instruction instr in method.Body.Instructions)
			{
				if (!(instr.Operand is MethodSpec))
				{
					continue;
				}
				MethodSpec spec = (MethodSpec)instr.Operand;
				if (this.ShouldInstantiate == null || this.ShouldInstantiate(spec))
				{
					if (!Instantiate(spec, out var instantiation))
					{
						onInstantiated(spec, instantiation);
					}
					instr.Operand = instantiation;
				}
			}
		}

		public bool Instantiate(MethodSpec methodSpec, out MethodDef def)
		{
			if (instantiations.TryGetValue(methodSpec, out def))
			{
				return true;
			}
			GenericArguments genericArguments = new GenericArguments();
			genericArguments.PushMethodArgs(methodSpec.GenericInstMethodSig.GenericArguments);
			MethodDef originDef = methodSpec.Method.ResolveMethodDefThrow();
			MethodSig newSig = ResolveMethod(originDef.MethodSig, genericArguments);
			newSig.Generic = false;
			newSig.GenParamCount = 0u;
			string newName = originDef.Name;
			foreach (TypeSig typeArg in methodSpec.GenericInstMethodSig.GenericArguments)
			{
				newName = newName + ";" + typeArg.TypeName;
			}
			def = new MethodDefUser(newName, newSig, originDef.ImplAttributes, originDef.Attributes);
			TypeSig thisParam = (originDef.HasThis ? originDef.Parameters[0].Type : null);
			def.DeclaringType2 = originDef.DeclaringType2;
			if (thisParam != null)
			{
				def.Parameters[0].Type = thisParam;
			}
			foreach (DeclSecurity declSec in originDef.DeclSecurities)
			{
				def.DeclSecurities.Add(declSec);
			}
			def.ImplMap = originDef.ImplMap;
			foreach (MethodOverride ov in originDef.Overrides)
			{
				def.Overrides.Add(ov);
			}
			def.Body = new CilBody();
			def.Body.InitLocals = originDef.Body.InitLocals;
			def.Body.MaxStack = originDef.Body.MaxStack;
			foreach (Local variable in originDef.Body.Variables)
			{
				Local newVar = new Local(variable.Type);
				def.Body.Variables.Add(newVar);
			}
			Dictionary<Instruction, Instruction> instrMap = new Dictionary<Instruction, Instruction>();
			foreach (Instruction instr2 in originDef.Body.Instructions)
			{
				Instruction newInstr = new Instruction(instr2.OpCode, ResolveOperand(instr2.Operand, genericArguments));
				def.Body.Instructions.Add(newInstr);
				instrMap[instr2] = newInstr;
			}
			foreach (Instruction instr in def.Body.Instructions)
			{
				if (instr.Operand is Instruction)
				{
					instr.Operand = instrMap[(Instruction)instr.Operand];
				}
				else if (instr.Operand is Instruction[])
				{
					Instruction[] targets = (Instruction[])((Instruction[])instr.Operand).Clone();
					for (int i = 0; i < targets.Length; i++)
					{
						targets[i] = instrMap[targets[i]];
					}
					instr.Operand = targets;
				}
			}
			def.Body.UpdateInstructionOffsets();
			foreach (ExceptionHandler eh in originDef.Body.ExceptionHandlers)
			{
				ExceptionHandler newEH = new ExceptionHandler(eh.HandlerType);
				newEH.TryStart = instrMap[eh.TryStart];
				newEH.HandlerStart = instrMap[eh.HandlerStart];
				if (eh.TryEnd != null)
				{
					newEH.TryEnd = instrMap[eh.TryEnd];
				}
				if (eh.HandlerEnd != null)
				{
					newEH.HandlerEnd = instrMap[eh.HandlerEnd];
				}
				if (eh.CatchType != null)
				{
					newEH.CatchType = genericArguments.Resolve(newEH.CatchType.ToTypeSig()).ToTypeDefOrRef();
				}
				else if (eh.FilterStart != null)
				{
					newEH.FilterStart = instrMap[eh.FilterStart];
				}
				def.Body.ExceptionHandlers.Add(newEH);
			}
			instantiations[methodSpec] = def;
			return false;
		}

		private FieldSig ResolveField(FieldSig sig, GenericArguments genericArgs)
		{
			FieldSig newSig = sig.Clone();
			newSig.Type = genericArgs.ResolveType(newSig.Type);
			return newSig;
		}

		private GenericInstMethodSig ResolveInst(GenericInstMethodSig sig, GenericArguments genericArgs)
		{
			GenericInstMethodSig newSig = sig.Clone();
			for (int i = 0; i < newSig.GenericArguments.Count; i++)
			{
				newSig.GenericArguments[i] = genericArgs.ResolveType(newSig.GenericArguments[i]);
			}
			return newSig;
		}

		private MethodSig ResolveMethod(MethodSig sig, GenericArguments genericArgs)
		{
			MethodSig newSig = sig.Clone();
			for (int j = 0; j < newSig.Params.Count; j++)
			{
				newSig.Params[j] = genericArgs.ResolveType(newSig.Params[j]);
			}
			if (newSig.ParamsAfterSentinel != null)
			{
				for (int i = 0; i < newSig.ParamsAfterSentinel.Count; i++)
				{
					newSig.ParamsAfterSentinel[i] = genericArgs.ResolveType(newSig.ParamsAfterSentinel[i]);
				}
			}
			newSig.RetType = genericArgs.ResolveType(newSig.RetType);
			return newSig;
		}

		private object ResolveOperand(object operand, GenericArguments genericArgs)
		{
			if (operand is MemberRef)
			{
				MemberRef memberRef = (MemberRef)operand;
				if (memberRef.IsFieldRef)
				{
					FieldSig field = ResolveField(memberRef.FieldSig, genericArgs);
					memberRef = new MemberRefUser(memberRef.Module, memberRef.Name, field, memberRef.Class);
				}
				else
				{
					MethodSig method = ResolveMethod(memberRef.MethodSig, genericArgs);
					memberRef = new MemberRefUser(memberRef.Module, memberRef.Name, method, memberRef.Class);
				}
				return memberRef;
			}
			if (operand is TypeSpec)
			{
				TypeSig sig = ((TypeSpec)operand).TypeSig;
				return genericArgs.ResolveType(sig).ToTypeDefOrRef();
			}
			if (operand is MethodSpec)
			{
				MethodSpec spec = (MethodSpec)operand;
				return new MethodSpecUser(spec.Method, ResolveInst(spec.GenericInstMethodSig, genericArgs));
			}
			return operand;
		}
	}
}
