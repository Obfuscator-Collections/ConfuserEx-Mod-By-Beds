using KoiVM.AST.IL;
using KoiVM.AST.IR;
using KoiVM.VMIR;

namespace KoiVM.VMIL.Translation
{
	public class ExitHandler : ITranslationHandler
	{
		public IROpCode IRCode => IROpCode.__EXIT;

		public void Translate(IRInstruction instr, ILTranslator tr)
		{
			tr.Instructions.Add(new ILInstruction(ILOpCode.__EXIT));
		}
	}
}
