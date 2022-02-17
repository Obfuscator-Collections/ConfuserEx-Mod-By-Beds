#define DEBUG
using System.Diagnostics;
using dnlib.DotNet.Emit;
using KoiVM.AST.ILAST;
using KoiVM.AST.IR;

namespace KoiVM.VMIR.Translation
{
	public class ShrHandler : ITranslationHandler
	{
		public Code ILCode => Code.Shr;

		public IIROperand Translate(ILASTExpression expr, IRTranslator tr)
		{
			Debug.Assert(expr.Arguments.Length == 2);
			IRVariable ret = tr.Context.AllocateVRegister(expr.Type.Value);
			tr.Instructions.Add(new IRInstruction(IROpCode.MOV)
			{
				Operand1 = ret,
				Operand2 = tr.Translate(expr.Arguments[0])
			});
			tr.Instructions.Add(new IRInstruction(IROpCode.SHR)
			{
				Operand1 = ret,
				Operand2 = tr.Translate(expr.Arguments[1])
			});
			return ret;
		}
	}
}
