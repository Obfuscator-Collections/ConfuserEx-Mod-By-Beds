#define DEBUG
using System.Diagnostics;
using dnlib.DotNet.Emit;
using KoiVM.AST;
using KoiVM.AST.ILAST;
using KoiVM.AST.IR;

namespace KoiVM.VMIR.Translation
{
	public class CltUnHandler : ITranslationHandler
	{
		public Code ILCode => Code.Clt_Un;

		public IIROperand Translate(ILASTExpression expr, IRTranslator tr)
		{
			Debug.Assert(expr.Arguments.Length == 2);
			tr.Instructions.Add(new IRInstruction(IROpCode.CMP)
			{
				Operand1 = tr.Translate(expr.Arguments[0]),
				Operand2 = tr.Translate(expr.Arguments[1])
			});
			IRVariable ret = tr.Context.AllocateVRegister(ASTType.I4);
			tr.Instructions.Add(new IRInstruction(IROpCode.__GETF)
			{
				Operand1 = ret,
				Operand2 = IRConstant.FromI4(1 << tr.Arch.Flags.CARRY)
			});
			return ret;
		}
	}
}
