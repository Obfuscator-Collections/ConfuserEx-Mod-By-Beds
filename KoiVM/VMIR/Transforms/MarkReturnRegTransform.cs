using KoiVM.AST;
using KoiVM.AST.IR;

namespace KoiVM.VMIR.Transforms
{
	public class MarkReturnRegTransform : ITransform
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
			InstrCallInfo callInfo = instr.Annotation as InstrCallInfo;
			if (callInfo != null && callInfo.ReturnValue != null)
			{
				if (instr.Operand1 is IRRegister && ((IRRegister)instr.Operand1).SourceVariable == callInfo.ReturnValue)
				{
					callInfo.ReturnRegister = (IRRegister)instr.Operand1;
				}
				else if (instr.Operand1 is IRPointer && ((IRPointer)instr.Operand1).SourceVariable == callInfo.ReturnValue)
				{
					callInfo.ReturnSlot = (IRPointer)instr.Operand1;
				}
			}
		}
	}
}
