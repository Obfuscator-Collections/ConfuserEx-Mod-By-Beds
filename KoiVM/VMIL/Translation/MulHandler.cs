using System;
using KoiVM.AST;
using KoiVM.AST.IL;
using KoiVM.AST.IR;
using KoiVM.VMIR;

namespace KoiVM.VMIL.Translation
{
	public class MulHandler : ITranslationHandler
	{
		public IROpCode IRCode => IROpCode.MUL;

		public void Translate(IRInstruction instr, ILTranslator tr)
		{
			tr.PushOperand(instr.Operand1);
			tr.PushOperand(instr.Operand2);
			switch (TypeInference.InferBinaryOp(instr.Operand1.Type, instr.Operand2.Type))
			{
			case ASTType.I4:
				tr.Instructions.Add(new ILInstruction(ILOpCode.MUL_DWORD));
				break;
			case ASTType.I8:
			case ASTType.Ptr:
			case ASTType.ByRef:
				tr.Instructions.Add(new ILInstruction(ILOpCode.MUL_QWORD));
				break;
			case ASTType.R4:
				tr.Instructions.Add(new ILInstruction(ILOpCode.MUL_R32));
				break;
			case ASTType.R8:
				tr.Instructions.Add(new ILInstruction(ILOpCode.MUL_R64));
				break;
			default:
				throw new NotSupportedException();
			}
			tr.PopOperand(instr.Operand1);
		}
	}
}
