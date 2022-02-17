#define DEBUG
using System;
using System.Diagnostics;
using dnlib.DotNet.Emit;
using KoiVM.AST;
using KoiVM.AST.ILAST;
using KoiVM.AST.IR;

namespace KoiVM.VMIR.Translation
{
	public class ConvU4Handler : ITranslationHandler
	{
		public Code ILCode => Code.Conv_U4;

		public IIROperand Translate(ILASTExpression expr, IRTranslator tr)
		{
			Debug.Assert(expr.Arguments.Length == 1);
			IIROperand value = tr.Translate(expr.Arguments[0]);
			ASTType valueType = value.Type;
			if (valueType == ASTType.I4)
			{
				return value;
			}
			IRVariable retVar = tr.Context.AllocateVRegister(ASTType.I4);
			switch (valueType)
			{
			case ASTType.I8:
			case ASTType.Ptr:
				tr.Instructions.Add(new IRInstruction(IROpCode.MOV, retVar, value));
				break;
			case ASTType.R4:
			case ASTType.R8:
			{
				IRVariable tmp = tr.Context.AllocateVRegister(ASTType.I8);
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
