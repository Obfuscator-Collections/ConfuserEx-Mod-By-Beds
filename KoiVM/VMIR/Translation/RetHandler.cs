#define DEBUG
using System.Diagnostics;
using dnlib.DotNet.Emit;
using KoiVM.AST.ILAST;
using KoiVM.AST.IR;
using KoiVM.VM;

namespace KoiVM.VMIR.Translation
{
	public class RetHandler : ITranslationHandler
	{
		public Code ILCode => Code.Ret;

		public IIROperand Translate(ILASTExpression expr, IRTranslator tr)
		{
			if (expr.Arguments.Length == 1)
			{
				IIROperand value = tr.Translate(expr.Arguments[0]);
				tr.Instructions.Add(new IRInstruction(IROpCode.MOV)
				{
					Operand1 = new IRRegister(VMRegisters.R0, value.Type),
					Operand2 = value
				});
			}
			else
			{
				Debug.Assert(expr.Arguments.Length == 0);
			}
			tr.Instructions.Add(new IRInstruction(IROpCode.RET));
			return null;
		}
	}
}
