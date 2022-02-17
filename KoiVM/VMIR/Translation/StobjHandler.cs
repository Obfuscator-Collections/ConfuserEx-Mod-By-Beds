#define DEBUG
using System.Diagnostics;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using KoiVM.AST;
using KoiVM.AST.ILAST;
using KoiVM.AST.IR;

namespace KoiVM.VMIR.Translation
{
	public class StobjHandler : ITranslationHandler
	{
		public Code ILCode => Code.Stobj;

		public IIROperand Translate(ILASTExpression expr, IRTranslator tr)
		{
			Debug.Assert(expr.Arguments.Length == 2);
			IIROperand addr = tr.Translate(expr.Arguments[0]);
			IIROperand value = tr.Translate(expr.Arguments[1]);
			tr.Instructions.Add(new IRInstruction(IROpCode.__STOBJ, addr, value)
			{
				Annotation = new PointerInfo("STOBJ", (ITypeDefOrRef)expr.Operand)
			});
			return null;
		}
	}
}
