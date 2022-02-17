using KoiVM.AST.IL;
using KoiVM.AST.IR;
using KoiVM.VMIR;

namespace KoiVM.VMIL.Translation
{
	public class LeaveHandler : ITranslationHandler
	{
		public IROpCode IRCode => IROpCode.LEAVE;

		public void Translate(IRInstruction instr, ILTranslator tr)
		{
			tr.PushOperand(instr.Operand1);
			tr.Instructions.Add(new ILInstruction(ILOpCode.LEAVE)
			{
				Annotation = instr.Annotation
			});
		}
	}
}
