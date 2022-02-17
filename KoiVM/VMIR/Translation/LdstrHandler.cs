using dnlib.DotNet.Emit;
using KoiVM.AST.ILAST;
using KoiVM.AST.IR;

namespace KoiVM.VMIR.Translation
{
	public class LdstrHandler : ITranslationHandler
	{
		public Code ILCode => Code.Ldstr;

		public IIROperand Translate(ILASTExpression expr, IRTranslator tr)
		{
			return IRConstant.FromString((string)expr.Operand);
		}
	}
}
