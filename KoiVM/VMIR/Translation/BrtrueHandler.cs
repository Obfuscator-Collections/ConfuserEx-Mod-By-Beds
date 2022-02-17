#define DEBUG
using System.Diagnostics;
using dnlib.DotNet.Emit;
using KoiVM.AST;
using KoiVM.AST.ILAST;
using KoiVM.AST.IR;
using KoiVM.CFG;

namespace KoiVM.VMIR.Translation
{
	public class BrtrueHandler : ITranslationHandler
	{
		public Code ILCode => Code.Brtrue;

		public IIROperand Translate(ILASTExpression expr, IRTranslator tr)
		{
			Debug.Assert(expr.Arguments.Length == 1);
			IIROperand val = tr.Translate(expr.Arguments[0]);
			TranslationHelpers.EmitCompareEq(tr, expr.Arguments[0].Type.Value, val, IRConstant.FromI4(0));
			IRVariable tmp = tr.Context.AllocateVRegister(ASTType.I4);
			tr.Instructions.Add(new IRInstruction(IROpCode.__GETF)
			{
				Operand1 = tmp,
				Operand2 = IRConstant.FromI4(1 << tr.Arch.Flags.ZERO)
			});
			tr.Instructions.Add(new IRInstruction(IROpCode.JZ)
			{
				Operand1 = new IRBlockTarget((IBasicBlock)expr.Operand),
				Operand2 = tmp
			});
			return null;
		}
	}
}
