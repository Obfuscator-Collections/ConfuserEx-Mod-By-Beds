using dnlib.DotNet;
using dnlib.DotNet.Emit;
using KoiVM.AST;
using KoiVM.AST.ILAST;
using KoiVM.AST.IR;

namespace KoiVM.VMIR.Translation
{
	public class LdargaHandler : ITranslationHandler
	{
		public Code ILCode => Code.Ldarga;

		public IIROperand Translate(ILASTExpression expr, IRTranslator tr)
		{
			IRVariable param = tr.Context.ResolveParameter((Parameter)expr.Operand);
			IRVariable ret = tr.Context.AllocateVRegister(ASTType.ByRef);
			tr.Instructions.Add(new IRInstruction(IROpCode.__LEA, ret, param));
			return ret;
		}
	}
}
