using KoiVM.AST.IR;
using KoiVM.VMIR;

namespace KoiVM.VMIL.Translation
{
	public class PushHandler : ITranslationHandler
	{
		public IROpCode IRCode => IROpCode.PUSH;

		public void Translate(IRInstruction instr, ILTranslator tr)
		{
			tr.PushOperand(instr.Operand1);
		}
	}
}
