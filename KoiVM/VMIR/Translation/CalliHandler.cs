using System;
using dnlib.DotNet.Emit;
using KoiVM.AST.ILAST;
using KoiVM.AST.IR;

namespace KoiVM.VMIR.Translation
{
	public class CalliHandler : ITranslationHandler
	{
		public Code ILCode => Code.Calli;

		public IIROperand Translate(ILASTExpression expr, IRTranslator tr)
		{
			throw new NotSupportedException();
		}
	}
}
