using KoiVM.AST.IR;
using KoiVM.VMIR;

namespace KoiVM.VMIL.Translation
{
	public class PopHandler : ITranslationHandler
	{
		public IROpCode IRCode => IROpCode.POP;

		public void Translate(IRInstruction instr, ILTranslator tr)
		{
			tr.PopOperand(instr.Operand1);
		}
	}
}
