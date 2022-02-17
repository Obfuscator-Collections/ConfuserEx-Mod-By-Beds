#define DEBUG
using System;
using System.Diagnostics;
using dnlib.DotNet.Emit;
using KoiVM.AST;
using KoiVM.AST.ILAST;
using KoiVM.AST.IR;

namespace KoiVM.VMIR.Translation
{
	public class ConvI8Handler : ITranslationHandler
	{
		public Code ILCode => Code.Conv_I8;

		public IIROperand Translate(ILASTExpression expr, IRTranslator tr)
		{
			Debug.Assert(expr.Arguments.Length == 1);
			IIROperand value = tr.Translate(expr.Arguments[0]);
			ASTType valueType = value.Type;
			if (valueType == ASTType.I8)
			{
				return value;
			}
			IRVariable retVar = tr.Context.AllocateVRegister(ASTType.I8);
			switch (valueType)
			{
			case ASTType.I4:
				tr.Instructions.Add(new IRInstruction(IROpCode.SX, retVar, value));
				break;
			case ASTType.Ptr:
				tr.Instructions.Add(new IRInstruction(IROpCode.MOV, retVar, value));
				break;
			case ASTType.R4:
			case ASTType.R8:
				tr.Instructions.Add(new IRInstruction(IROpCode.ICONV, retVar, value));
				break;
			default:
				throw new NotSupportedException();
			}
			return retVar;
		}
	}
}
