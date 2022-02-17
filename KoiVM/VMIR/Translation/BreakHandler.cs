using dnlib.DotNet.Emit;
using KoiVM.AST.ILAST;
using KoiVM.AST.IR;

namespace KoiVM.VMIR.Translation
{
	public class BreakHandler : ITranslationHandler
	{
		public Code ILCode => Code.Break;

		public IIROperand Translate(ILASTExpression expr, IRTranslator tr)
		{
			int ecallId = tr.VM.Runtime.VMCall.BREAK;
			tr.Instructions.Add(new IRInstruction(IROpCode.VCALL, IRConstant.FromI4(ecallId)));
			return null;
		}
	}
}
