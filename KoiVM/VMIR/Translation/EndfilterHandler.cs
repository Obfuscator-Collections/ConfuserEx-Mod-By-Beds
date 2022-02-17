#define DEBUG
using System.Diagnostics;
using dnlib.DotNet.Emit;
using KoiVM.AST.ILAST;
using KoiVM.AST.IR;
using KoiVM.CFG;

namespace KoiVM.VMIR.Translation
{
	public class EndfilterHandler : ITranslationHandler
	{
		public Code ILCode => Code.Endfilter;

		public IIROperand Translate(ILASTExpression expr, IRTranslator tr)
		{
			Debug.Assert(expr.Arguments.Length == 1);
			tr.Instructions.Add(new IRInstruction(IROpCode.__EHRET, tr.Translate(expr.Arguments[0])));
			tr.Block.Flags |= BlockFlags.ExitEHReturn;
			return null;
		}
	}
}
