using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using KoiVM.VM;
using KoiVM.VMIL;

namespace KoiVM.RT.Mutation
{
	public class RTConstants
	{
		private readonly Dictionary<string, int> constants = new Dictionary<string, int>();

		private void AddField(string fieldName, int fieldValue)
		{
			constants[fieldName] = fieldValue;
		}

		private void Conclude(Random random, IList<Instruction> instrs, TypeDef constType)
		{
			List<KeyValuePair<string, int>> constValues = constants.ToList();
			random.Shuffle(constValues);
			foreach (KeyValuePair<string, int> c in constValues)
			{
				instrs.Add(new Instruction(OpCodes.Ldnull));
				instrs.Add(new Instruction(OpCodes.Ldc_I4, c.Value));
				instrs.Add(new Instruction(OpCodes.Stfld, constType.FindField(RTMap.VMConstMap[c.Key])));
			}
		}

		public int? GetConstant(string name)
		{
			if (!constants.TryGetValue(name, out var ret))
			{
				return null;
			}
			return ret;
		}

		public void InjectConstants(ModuleDef rtModule, VMDescriptor desc, RuntimeHelpers helpers)
		{
			TypeDef constants = rtModule.Find(RTMap.VMConstants, isReflectionName: true);
			MethodDef cctor = constants.FindOrCreateStaticConstructor();
			IList<Instruction> instrs = cctor.Body.Instructions;
			instrs.Clear();
			for (int i = 0; i < 16; i++)
			{
				VMRegisters reg = (VMRegisters)i;
				byte regId = desc.Architecture.Registers[reg];
				string regField = reg.ToString();
				AddField(regField, regId);
			}
			for (int j = 0; j < 8; j++)
			{
				VMFlags fl = (VMFlags)j;
				int flId = desc.Architecture.Flags[fl];
				string flField = fl.ToString();
				AddField(flField, 1 << flId);
			}
			for (int k = 0; k < 68; k++)
			{
				ILOpCode op = (ILOpCode)k;
				byte opId = desc.Architecture.OpCodes[op];
				string opField = op.ToString();
				AddField(opField, opId);
			}
			for (int l = 0; l < 17; l++)
			{
				VMCalls vc = (VMCalls)l;
				int vcId = desc.Runtime.VMCall[vc];
				string vcField = vc.ToString();
				AddField(vcField, vcId);
			}
			AddField(ConstantFields.E_CALL.ToString(), (int)desc.Runtime.VCallOps.ECALL_CALL);
			AddField(ConstantFields.E_CALLVIRT.ToString(), (int)desc.Runtime.VCallOps.ECALL_CALLVIRT);
			AddField(ConstantFields.E_NEWOBJ.ToString(), (int)desc.Runtime.VCallOps.ECALL_NEWOBJ);
			AddField(ConstantFields.E_CALLVIRT_CONSTRAINED.ToString(), (int)desc.Runtime.VCallOps.ECALL_CALLVIRT_CONSTRAINED);
			AddField(ConstantFields.INIT.ToString(), (int)helpers.INIT);
			AddField(ConstantFields.INSTANCE.ToString(), desc.Runtime.RTFlags.INSTANCE);
			AddField(ConstantFields.CATCH.ToString(), desc.Runtime.RTFlags.EH_CATCH);
			AddField(ConstantFields.FILTER.ToString(), desc.Runtime.RTFlags.EH_FILTER);
			AddField(ConstantFields.FAULT.ToString(), desc.Runtime.RTFlags.EH_FAULT);
			AddField(ConstantFields.FINALLY.ToString(), desc.Runtime.RTFlags.EH_FINALLY);
			Conclude(desc.Random, instrs, constants);
			instrs.Add(Instruction.Create(OpCodes.Ret));
			cctor.Body.OptimizeMacros();
		}
	}
}
