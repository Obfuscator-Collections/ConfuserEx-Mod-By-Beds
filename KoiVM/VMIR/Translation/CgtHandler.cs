#define DEBUG
using System.Diagnostics;
using dnlib.DotNet.Emit;
using KoiVM.AST;
using KoiVM.AST.ILAST;
using KoiVM.AST.IR;

namespace KoiVM.VMIR.Translation
{
	public class CgtHandler : ITranslationHandler
	{
		public Code ILCode => Code.Cgt;

		public IIROperand Translate(ILASTExpression expr, IRTranslator tr)
		{
			Debug.Assert(expr.Arguments.Length == 2);
			tr.Instructions.Add(new IRInstruction(IROpCode.CMP)
			{
				Operand1 = tr.Translate(expr.Arguments[0]),
				Operand2 = tr.Translate(expr.Arguments[1])
			});
			IRVariable ret = tr.Context.AllocateVRegister(ASTType.I4);
			IRVariable fl = tr.Context.AllocateVRegister(ASTType.I4);
			tr.Instructions.Add(new IRInstruction(IROpCode.__GETF)
			{
				Operand1 = fl,
				Operand2 = IRConstant.FromI4((1 << tr.Arch.Flags.OVERFLOW) | (1 << tr.Arch.Flags.SIGN) | (1 << tr.Arch.Flags.ZERO))
			});
			tr.Instructions.Add(new IRInstruction(IROpCode.MOV)
			{
				Operand1 = ret,
				Operand2 = fl
			});
			TranslationHelpers.EmitCompareEq(tr, ASTType.I4, ret, IRConstant.FromI4((1 << tr.Arch.Flags.OVERFLOW) | (1 << tr.Arch.Flags.SIGN)));
			tr.Instructions.Add(new IRInstruction(IROpCode.__GETF)
			{
				Operand1 = ret,
				Operand2 = IRConstant.FromI4(1 << tr.Arch.Flags.ZERO)
			});
			tr.Instructions.Add(new IRInstruction(IROpCode.__AND)
			{
				Operand1 = fl,
				Operand2 = fl
			});
			tr.Instructions.Add(new IRInstruction(IROpCode.__GETF)
			{
				Operand1 = fl,
				Operand2 = IRConstant.FromI4(1 << tr.Arch.Flags.ZERO)
			});
			tr.Instructions.Add(new IRInstruction(IROpCode.__OR)
			{
				Operand1 = ret,
				Operand2 = fl
			});
			return ret;
		}
	}
}
