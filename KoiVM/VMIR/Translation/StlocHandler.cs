#define DEBUG
using System.Diagnostics;
using dnlib.DotNet.Emit;
using KoiVM.AST.ILAST;
using KoiVM.AST.IR;

namespace KoiVM.VMIR.Translation
{
	public class StlocHandler : ITranslationHandler
	{
		public Code ILCode => Code.Stloc;

		public IIROperand Translate(ILASTExpression expr, IRTranslator tr)
		{
			Debug.Assert(expr.Arguments.Length == 1);
			tr.Instructions.Add(new IRInstruction(IROpCode.MOV)
			{
				Operand1 = tr.Context.ResolveLocal((Local)expr.Operand),
				Operand2 = tr.Translate(expr.Arguments[0])
			});
			return null;
		}
	}
}
