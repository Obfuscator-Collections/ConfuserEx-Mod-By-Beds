#define DEBUG
using System.Diagnostics;
using dnlib.DotNet.Emit;
using KoiVM.AST.ILAST;
using KoiVM.AST.IR;

namespace KoiVM.VMIR.Translation
{
	public class ThrowHandler : ITranslationHandler
	{
		public Code ILCode => Code.Throw;

		public IIROperand Translate(ILASTExpression expr, IRTranslator tr)
		{
			Debug.Assert(expr.Arguments.Length == 1);
			int ecallId = tr.VM.Runtime.VMCall.THROW;
			tr.Instructions.Add(new IRInstruction(IROpCode.PUSH, tr.Translate(expr.Arguments[0])));
			tr.Instructions.Add(new IRInstruction(IROpCode.VCALL)
			{
				Operand1 = IRConstant.FromI4(ecallId),
				Operand2 = IRConstant.FromI4(0)
			});
			return null;
		}
	}
}
