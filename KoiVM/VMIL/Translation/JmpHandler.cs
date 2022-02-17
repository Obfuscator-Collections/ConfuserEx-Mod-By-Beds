using KoiVM.AST;
using KoiVM.AST.IL;
using KoiVM.AST.IR;
using KoiVM.VMIR;

namespace KoiVM.VMIL.Translation
{
	public class JmpHandler : ITranslationHandler
	{
		public IROpCode IRCode => IROpCode.JMP;

		public void Translate(IRInstruction instr, ILTranslator tr)
		{
			tr.PushOperand(instr.Operand1);
			tr.Instructions.Add(new ILInstruction(ILOpCode.JMP)
			{
				Annotation = InstrAnnotation.JUMP
			});
		}
	}
}
