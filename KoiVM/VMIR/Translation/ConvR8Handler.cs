#define DEBUG
using System;
using System.Diagnostics;
using dnlib.DotNet.Emit;
using KoiVM.AST;
using KoiVM.AST.ILAST;
using KoiVM.AST.IR;

namespace KoiVM.VMIR.Translation
{
	public class ConvR8Handler : ITranslationHandler
	{
		public Code ILCode => Code.Conv_R8;

		public IIROperand Translate(ILASTExpression expr, IRTranslator tr)
		{
			Debug.Assert(expr.Arguments.Length == 1);
			IIROperand value = tr.Translate(expr.Arguments[0]);
			ASTType valueType = value.Type;
			if (valueType == ASTType.R8)
			{
				return value;
			}
			IRVariable retVar = tr.Context.AllocateVRegister(ASTType.R8);
			switch (valueType)
			{
			case ASTType.I4:
			{
				IRVariable tmpVar = tr.Context.AllocateVRegister(ASTType.I8);
				tr.Instructions.Add(new IRInstruction(IROpCode.SX, tmpVar, value));
				tr.Instructions.Add(new IRInstruction(IROpCode.FCONV, retVar, tmpVar));
				break;
			}
			case ASTType.I8:
				tr.Instructions.Add(new IRInstruction(IROpCode.FCONV, retVar, value));
				break;
			case ASTType.R4:
				tr.Instructions.Add(new IRInstruction(IROpCode.FCONV, retVar, value));
				break;
			default:
				throw new NotSupportedException();
			}
			return retVar;
		}
	}
}
