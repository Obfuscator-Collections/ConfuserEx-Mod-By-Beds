using dnlib.DotNet.Emit;
using KoiVM.AST.ILAST;
using KoiVM.AST.IR;

namespace KoiVM.VMIR.Translation
{
	public class LdcR4Handler : ITranslationHandler
	{
		public Code ILCode => Code.Ldc_R4;

		public IIROperand Translate(ILASTExpression expr, IRTranslator tr)
		{
			return IRConstant.FromR4((float)expr.Operand);
		}
	}
}
