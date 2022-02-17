#define DEBUG
using System.Diagnostics;
using dnlib.DotNet.Emit;
using KoiVM.AST.ILAST;
using KoiVM.AST.IR;

namespace KoiVM.VMIR.Translation
{
	public class ConvOvfU8UnHandler : ITranslationHandler
	{
		public Code ILCode => Code.Conv_Ovf_U8_Un;

		public IIROperand Translate(ILASTExpression expr, IRTranslator tr)
		{
			Debug.Assert(expr.Arguments.Length == 1);
			return tr.Translate(expr.Arguments[0]);
		}
	}
}
