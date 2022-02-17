using KoiVM.AST.IR;

namespace KoiVM.VMIR.Transforms
{
	public class LogicTransform : ITransform
	{
		public void Initialize(IRTransformer tr)
		{
		}

		public void Transform(IRTransformer tr)
		{
			tr.Instructions.VisitInstrs(VisitInstr, tr);
		}

		private void VisitInstr(IRInstrList instrs, IRInstruction instr, ref int index, IRTransformer tr)
		{
			if (instr.OpCode == IROpCode.__NOT)
			{
				instrs.Replace(index, new IRInstruction[1]
				{
					new IRInstruction(IROpCode.NOR, instr.Operand1, instr.Operand1, instr)
				});
			}
			else if (instr.OpCode == IROpCode.__AND)
			{
				IRVariable tmp = tr.Context.AllocateVRegister(instr.Operand2.Type);
				instrs.Replace(index, new IRInstruction[4]
				{
					new IRInstruction(IROpCode.MOV, tmp, instr.Operand2, instr),
					new IRInstruction(IROpCode.NOR, instr.Operand1, instr.Operand1, instr),
					new IRInstruction(IROpCode.NOR, tmp, tmp, instr),
					new IRInstruction(IROpCode.NOR, instr.Operand1, tmp, instr)
				});
			}
			else if (instr.OpCode == IROpCode.__OR)
			{
				instrs.Replace(index, new IRInstruction[2]
				{
					new IRInstruction(IROpCode.NOR, instr.Operand1, instr.Operand2, instr),
					new IRInstruction(IROpCode.NOR, instr.Operand1, instr.Operand1, instr)
				});
			}
			else if (instr.OpCode == IROpCode.__XOR)
			{
				IRVariable tmp2 = tr.Context.AllocateVRegister(instr.Operand2.Type);
				IRVariable tmp3 = tr.Context.AllocateVRegister(instr.Operand2.Type);
				instrs.Replace(index, new IRInstruction[7]
				{
					new IRInstruction(IROpCode.MOV, tmp2, instr.Operand1, instr),
					new IRInstruction(IROpCode.NOR, tmp2, instr.Operand2, instr),
					new IRInstruction(IROpCode.MOV, tmp3, instr.Operand2, instr),
					new IRInstruction(IROpCode.NOR, instr.Operand1, instr.Operand1, instr),
					new IRInstruction(IROpCode.NOR, tmp3, tmp3, instr),
					new IRInstruction(IROpCode.NOR, instr.Operand1, tmp3, instr),
					new IRInstruction(IROpCode.NOR, instr.Operand1, tmp2, instr)
				});
			}
		}
	}
}
