using KoiVM.AST.IR;
using KoiVM.VMIR;

namespace KoiVM.VMIL.Translation
{
	public class MovHandler : ITranslationHandler
	{
		public IROpCode IRCode => IROpCode.MOV;

		public void Translate(IRInstruction instr, ILTranslator tr)
		{
			tr.PushOperand(instr.Operand2);
			tr.PopOperand(instr.Operand1);
		}
	}
}
