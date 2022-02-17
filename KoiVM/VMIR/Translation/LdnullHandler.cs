using dnlib.DotNet.Emit;
using KoiVM.AST.ILAST;
using KoiVM.AST.IR;

namespace KoiVM.VMIR.Translation
{
	public class LdnullHandler : ITranslationHandler
	{
		public Code ILCode => Code.Ldnull;

		public IIROperand Translate(ILASTExpression expr, IRTranslator tr)
		{
			return IRConstant.Null();
		}
	}
}
