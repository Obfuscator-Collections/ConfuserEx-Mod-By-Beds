using KoiVM.AST.IL;
using KoiVM.AST.IR;
using KoiVM.VMIR;

namespace KoiVM.VMIL.Translation
{
	public class EndCallHandler : ITranslationHandler
	{
		public IROpCode IRCode => IROpCode.__ENDCALL;

		public void Translate(IRInstruction instr, ILTranslator tr)
		{
			tr.Instructions.Add(new ILInstruction(ILOpCode.__ENDCALL)
			{
				Annotation = instr.Annotation
			});
		}
	}
}
