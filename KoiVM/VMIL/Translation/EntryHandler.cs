using KoiVM.AST.IL;
using KoiVM.AST.IR;
using KoiVM.VMIR;

namespace KoiVM.VMIL.Translation
{
	public class EntryHandler : ITranslationHandler
	{
		public IROpCode IRCode => IROpCode.__ENTRY;

		public void Translate(IRInstruction instr, ILTranslator tr)
		{
			tr.Instructions.Add(new ILInstruction(ILOpCode.__ENTRY));
		}
	}
}
