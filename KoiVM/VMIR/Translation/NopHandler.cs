using dnlib.DotNet.Emit;
using KoiVM.AST.ILAST;
using KoiVM.AST.IR;

namespace KoiVM.VMIR.Translation
{
	public class NopHandler : ITranslationHandler
	{
		public Code ILCode => Code.Nop;

		public IIROperand Translate(ILASTExpression expr, IRTranslator tr)
		{
			tr.Instructions.Add(new IRInstruction(IROpCode.NOP));
			return null;
		}
	}
}
