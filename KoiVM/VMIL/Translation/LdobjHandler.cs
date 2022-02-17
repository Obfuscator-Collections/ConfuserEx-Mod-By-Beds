using dnlib.DotNet;
using KoiVM.AST;
using KoiVM.AST.IL;
using KoiVM.AST.IR;
using KoiVM.VMIR;

namespace KoiVM.VMIL.Translation
{
	public class LdobjHandler : ITranslationHandler
	{
		public IROpCode IRCode => IROpCode.__LDOBJ;

		public void Translate(IRInstruction instr, ILTranslator tr)
		{
			tr.PushOperand(instr.Operand1);
			TypeSig rawType = ((PointerInfo)instr.Annotation).PointerType.ToTypeSig();
			tr.Instructions.Add(new ILInstruction(TranslationHelpers.GetLIND(instr.Operand2.Type, rawType)));
			tr.PopOperand(instr.Operand2);
		}
	}
}
