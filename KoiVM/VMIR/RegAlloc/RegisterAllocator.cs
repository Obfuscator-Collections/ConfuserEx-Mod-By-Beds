#define DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using KoiVM.AST.IR;
using KoiVM.CFG;
using KoiVM.VM;

namespace KoiVM.VMIR.RegAlloc
{
	public class RegisterAllocator
	{
		private struct StackSlot
		{
			public readonly int Offset;

			public readonly IRVariable Variable;

			public StackSlot(int offset, IRVariable var)
			{
				Offset = offset;
				Variable = var;
			}
		}

		private class RegisterPool
		{
			private const int NumRegisters = 8;

			private IRVariable[] regAlloc;

			private Dictionary<IRVariable, StackSlot> spillVars;

			public int SpillOffset { get; set; }

			private static VMRegisters ToRegister(int regId)
			{
				return (VMRegisters)regId;
			}

			private static int FromRegister(VMRegisters reg)
			{
				return (int)reg;
			}

			public static RegisterPool Create(int baseOffset, Dictionary<IRVariable, StackSlot> globalVars)
			{
				RegisterPool pool = new RegisterPool();
				pool.regAlloc = new IRVariable[8];
				pool.spillVars = new Dictionary<IRVariable, StackSlot>(globalVars);
				pool.SpillOffset = baseOffset;
				return pool;
			}

			public VMRegisters? Allocate(IRVariable var)
			{
				for (int i = 0; i < regAlloc.Length; i++)
				{
					if (regAlloc[i] == null)
					{
						regAlloc[i] = var;
						return ToRegister(i);
					}
				}
				return null;
			}

			public void Deallocate(IRVariable var, VMRegisters reg)
			{
				Debug.Assert(regAlloc[FromRegister(reg)] == var);
				regAlloc[FromRegister(reg)] = null;
			}

			public void CheckLiveness(HashSet<IRVariable> live)
			{
				for (int i = 0; i < regAlloc.Length; i++)
				{
					if (regAlloc[i] != null && !live.Contains(regAlloc[i]))
					{
						regAlloc[i].Annotation = null;
						regAlloc[i] = null;
					}
				}
			}

			public StackSlot SpillVariable(IRVariable var)
			{
				StackSlot slot = new StackSlot(SpillOffset++, var);
				spillVars[var] = slot;
				return slot;
			}

			public StackSlot? CheckSpill(IRVariable var)
			{
				if (!spillVars.TryGetValue(var, out var ret))
				{
					return null;
				}
				return ret;
			}
		}

		private Dictionary<IRVariable, object> allocation;

		private int baseOffset;

		private Dictionary<IRVariable, StackSlot> globalVars;

		private Dictionary<BasicBlock<IRInstrList>, BlockLiveness> liveness;

		private readonly IRTransformer transformer;

		public int LocalSize { get; set; }

		public RegisterAllocator(IRTransformer transformer)
		{
			this.transformer = transformer;
		}

		public void Initialize()
		{
			List<BasicBlock<IRInstrList>> blocks = transformer.RootScope.GetBasicBlocks().Cast<BasicBlock<IRInstrList>>().ToList();
			liveness = LivenessAnalysis.ComputeLiveness(blocks);
			HashSet<IRVariable> stackVars = new HashSet<IRVariable>();
			foreach (KeyValuePair<BasicBlock<IRInstrList>, BlockLiveness> blockLiveness in liveness)
			{
				foreach (IRInstruction instr in blockLiveness.Key.Content)
				{
					if (instr.OpCode == IROpCode.__LEA)
					{
						IRVariable variable = (IRVariable)instr.Operand2;
						if (variable.VariableType != IRVariableType.Argument)
						{
							stackVars.Add(variable);
						}
					}
				}
				stackVars.UnionWith(blockLiveness.Value.OutLive);
			}
			int offset = 1;
			globalVars = stackVars.ToDictionary((IRVariable var) => var, (IRVariable var) => new StackSlot(offset++, var));
			baseOffset = offset;
			LocalSize = baseOffset - 1;
			offset = -2;
			IRVariable[] parameters = transformer.Context.GetParameters();
			for (int i = parameters.Length - 1; i >= 0; i--)
			{
				IRVariable paramVar = parameters[i];
				globalVars[paramVar] = new StackSlot(offset--, paramVar);
			}
			allocation = ((IEnumerable<KeyValuePair<IRVariable, StackSlot>>)globalVars).ToDictionary((Func<KeyValuePair<IRVariable, StackSlot>, IRVariable>)((KeyValuePair<IRVariable, StackSlot> pair) => pair.Key), (Func<KeyValuePair<IRVariable, StackSlot>, object>)((KeyValuePair<IRVariable, StackSlot> pair) => pair.Value));
		}

		public void Allocate(BasicBlock<IRInstrList> block)
		{
			BlockLiveness blockLiveness = liveness[block];
			Dictionary<IRInstruction, HashSet<IRVariable>> instrLiveness = LivenessAnalysis.ComputeLiveness(block, blockLiveness);
			RegisterPool pool = RegisterPool.Create(baseOffset, globalVars);
			for (int i = 0; i < block.Content.Count; i++)
			{
				IRInstruction instr = block.Content[i];
				pool.CheckLiveness(instrLiveness[instr]);
				if (instr.Operand1 != null)
				{
					instr.Operand1 = AllocateOperand(instr.Operand1, pool);
				}
				if (instr.Operand2 != null)
				{
					instr.Operand2 = AllocateOperand(instr.Operand2, pool);
				}
			}
			if (pool.SpillOffset - 1 > LocalSize)
			{
				LocalSize = pool.SpillOffset - 1;
			}
			baseOffset = pool.SpillOffset;
		}

		private IIROperand AllocateOperand(IIROperand operand, RegisterPool pool)
		{
			if (operand is IRVariable)
			{
				IRVariable variable = (IRVariable)operand;
				StackSlot? slot;
				VMRegisters? reg = AllocateVariable(pool, variable, out slot);
				if (reg.HasValue)
				{
					return new IRRegister(reg.Value)
					{
						SourceVariable = variable,
						Type = variable.Type
					};
				}
				variable.Annotation = slot.Value;
				return new IRPointer
				{
					Register = IRRegister.BP,
					Offset = slot.Value.Offset,
					SourceVariable = variable,
					Type = variable.Type
				};
			}
			return operand;
		}

		private VMRegisters? AllocateVariable(RegisterPool pool, IRVariable var, out StackSlot? stackSlot)
		{
			stackSlot = pool.CheckSpill(var);
			if (!stackSlot.HasValue)
			{
				VMRegisters? allocReg = ((var.Annotation == null) ? null : new VMRegisters?((VMRegisters)var.Annotation));
				if (!allocReg.HasValue)
				{
					allocReg = pool.Allocate(var);
				}
				if (allocReg.HasValue)
				{
					if (var.Annotation == null)
					{
						var.Annotation = allocReg.Value;
					}
					return allocReg;
				}
				stackSlot = pool.SpillVariable(var);
			}
			return null;
		}
	}
}
