using KoiVM.AST.IL;
using KoiVM.AST.IR;
using KoiVM.VMIR;

namespace KoiVM.VMIL.Translation
{
	public class BeginCallHandler : ITranslationHandler
	{
		public IROpCode IRCode => IROpCode.__BEGINCALL;

		public void Translate(IRInstruction instr, ILTranslator tr)
		{
			tr.Instructions.Add(new ILInstruction(ILOpCode.__BEGINCALL)
			{
				Annotation = instr.Annotation
			});
		}
	}
}
