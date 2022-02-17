#define DEBUG
using System.Diagnostics;
using dnlib.DotNet.Emit;
using KoiVM.AST.ILAST;
using KoiVM.AST.IR;
using KoiVM.CFG;

namespace KoiVM.VMIR.Translation
{
	public class SwitchHandler : ITranslationHandler
	{
		public Code ILCode => Code.Switch;

		public IIROperand Translate(ILASTExpression expr, IRTranslator tr)
		{
			Debug.Assert(expr.Arguments.Length == 1);
			IIROperand val = tr.Translate(expr.Arguments[0]);
			tr.Instructions.Add(new IRInstruction(IROpCode.SWT)
			{
				Operand1 = new IRJumpTable((IBasicBlock[])expr.Operand),
				Operand2 = val
			});
			return null;
		}
	}
}
