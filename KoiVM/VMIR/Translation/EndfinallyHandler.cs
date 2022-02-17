using dnlib.DotNet.Emit;
using KoiVM.AST.ILAST;
using KoiVM.AST.IR;
using KoiVM.CFG;

namespace KoiVM.VMIR.Translation
{
	public class EndfinallyHandler : ITranslationHandler
	{
		public Code ILCode => Code.Endfinally;

		public IIROperand Translate(ILASTExpression expr, IRTranslator tr)
		{
			tr.Instructions.Add(new IRInstruction(IROpCode.__EHRET));
			tr.Block.Flags |= BlockFlags.ExitEHReturn;
			return null;
		}
	}
}
