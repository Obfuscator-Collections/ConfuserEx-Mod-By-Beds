#define DEBUG
using System;
using System.Diagnostics;
using dnlib.DotNet.Emit;
using KoiVM.AST;
using KoiVM.AST.ILAST;
using KoiVM.AST.IR;

namespace KoiVM.VMIR.Translation
{
	public class ConvOvfUHandler : ITranslationHandler
	{
		public Code ILCode => Code.Conv_Ovf_U;

		public IIROperand Translate(ILASTExpression expr, IRTranslator tr)
		{
			Debug.Assert(expr.Arguments.Length == 1);
			IIROperand value = tr.Translate(expr.Arguments[0]);
			ASTType valueType = value.Type;
			if (valueType == ASTType.Ptr || valueType == ASTType.I4)
			{
				return value;
			}
			IRVariable retVar = tr.Context.AllocateVRegister(ASTType.Ptr);
			switch (valueType)
			{
			case ASTType.I8:
				tr.Instructions.Add(new IRInstruction(IROpCode.MOV, retVar, value));
				break;
			case ASTType.R4:
			case ASTType.R8:
			{
				IRVariable tmp = tr.Context.AllocateVRegister(ASTType.I8);
				tr.Instructions.Add(new IRInstruction(IROpCode.__SETF)
				{
					Operand1 = IRConstant.FromI4(1 << tr.Arch.Flags.UNSIGNED)
				});
				tr.Instructions.Add(new IRInstruction(IROpCode.ICONV, tmp, value));
				tr.Instructions.Add(new IRInstruction(IROpCode.MOV, retVar, tmp));
				break;
			}
			default:
				throw new NotSupportedException();
			}
			return retVar;
		}
	}
}
