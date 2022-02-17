using KoiVM.AST;
using KoiVM.AST.IL;
using KoiVM.AST.IR;
using KoiVM.VMIR;

namespace KoiVM.VMIL.Translation
{
	public class JnzHandler : ITranslationHandler
	{
		public IROpCode IRCode => IROpCode.JNZ;

		public void Translate(IRInstruction instr, ILTranslator tr)
		{
			tr.PushOperand(instr.Operand2);
			tr.PushOperand(instr.Operand1);
			tr.Instructions.Add(new ILInstruction(ILOpCode.JNZ)
			{
				Annotation = InstrAnnotation.JUMP
			});
		}
	}
}
