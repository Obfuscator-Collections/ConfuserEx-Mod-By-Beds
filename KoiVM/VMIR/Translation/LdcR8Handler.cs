using dnlib.DotNet.Emit;
using KoiVM.AST.ILAST;
using KoiVM.AST.IR;

namespace KoiVM.VMIR.Translation
{
	public class LdcR8Handler : ITranslationHandler
	{
		public Code ILCode => Code.Ldc_R8;

		public IIROperand Translate(ILASTExpression expr, IRTranslator tr)
		{
			return IRConstant.FromR8((double)expr.Operand);
		}
	}
}
