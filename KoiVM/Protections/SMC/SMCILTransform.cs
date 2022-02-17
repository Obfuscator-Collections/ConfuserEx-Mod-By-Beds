using System.Linq;
using KoiVM.AST.IL;
using KoiVM.CFG;
using KoiVM.VMIL;

namespace KoiVM.Protections.SMC
{
	internal class SMCILTransform : ITransform
	{
		private int adrKey;

		private SMCBlock newTrampoline;

		private ILBlock trampoline;

		public void Initialize(ILTransformer tr)
		{
			trampoline = null;
			tr.RootScope.ProcessBasicBlocks(delegate(BasicBlock<ILInstrList> b)
			{
				if (b.Content.Any((ILInstruction instr) => instr.IR != null && instr.IR.Annotation == SMCBlock.AddressPart2))
				{
					trampoline = (ILBlock)b;
				}
			});
			if (trampoline != null)
			{
				ScopeBlock scope = tr.RootScope.SearchBlock(trampoline).Last();
				newTrampoline = new SMCBlock(trampoline.Id, trampoline.Content);
				scope.Content[scope.Content.IndexOf(trampoline)] = newTrampoline;
				adrKey = tr.VM.Random.Next();
				newTrampoline.Key = (byte)tr.VM.Random.Next();
			}
		}

		public void Transform(ILTransformer tr)
		{
			if (tr.Block.Targets.Contains(trampoline))
			{
				tr.Block.Targets[tr.Block.Targets.IndexOf(trampoline)] = newTrampoline;
			}
			if (tr.Block.Sources.Contains(trampoline))
			{
				tr.Block.Sources[tr.Block.Sources.IndexOf(trampoline)] = newTrampoline;
			}
			tr.Instructions.VisitInstrs(VisitInstr, tr);
		}

		private void VisitInstr(ILInstrList instrs, ILInstruction instr, ref int index, ILTransformer tr)
		{
			if (instr.Operand is ILBlockTarget)
			{
				ILBlockTarget target = (ILBlockTarget)instr.Operand;
				if (target.Target == trampoline)
				{
					target.Target = newTrampoline;
				}
			}
			else if (instr.IR == null)
			{
				return;
			}
			if (instr.IR.Annotation == SMCBlock.CounterInit && instr.OpCode == ILOpCode.PUSHI_DWORD)
			{
				ILImmediate imm2 = (ILImmediate)instr.Operand;
				if ((int)imm2.Value == 251658241)
				{
					newTrampoline.CounterOperand = imm2;
				}
			}
			else if (instr.IR.Annotation == SMCBlock.EncryptionKey && instr.OpCode == ILOpCode.PUSHI_DWORD)
			{
				ILImmediate imm3 = (ILImmediate)instr.Operand;
				if ((int)imm3.Value == 251658242)
				{
					imm3.Value = (int)newTrampoline.Key;
				}
			}
			else if (instr.IR.Annotation == SMCBlock.AddressPart1 && instr.OpCode == ILOpCode.PUSHI_DWORD && instr.Operand is ILBlockTarget)
			{
				ILBlockTarget target2 = (ILBlockTarget)instr.Operand;
				ILInstruction relBase = new ILInstruction(ILOpCode.PUSHR_QWORD, ILRegister.IP, instr);
				instr.OpCode = ILOpCode.PUSHI_DWORD;
				instr.Operand = new SMCBlockRef(target2, relBase, (uint)adrKey);
				instrs.Replace(index, new ILInstruction[3]
				{
					relBase,
					instr,
					new ILInstruction(ILOpCode.ADD_QWORD, null, instr)
				});
			}
			else if (instr.IR.Annotation == SMCBlock.AddressPart2 && instr.OpCode == ILOpCode.PUSHI_DWORD)
			{
				ILImmediate imm = (ILImmediate)instr.Operand;
				if ((int)imm.Value == 251658243)
				{
					imm.Value = adrKey;
				}
			}
		}
	}
}
