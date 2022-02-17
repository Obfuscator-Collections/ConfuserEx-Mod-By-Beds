using dnlib.DotNet;
using dnlib.DotNet.Emit;
using KoiVM.AST.ILAST;
using KoiVM.AST.IR;

namespace KoiVM.VMIR.Translation
{
	public class SizeofHandler : ITranslationHandler
	{
		public Code ILCode => Code.Sizeof;

		public IIROperand Translate(ILASTExpression expr, IRTranslator tr)
		{
			int typeId = (int)tr.Runtime.Descriptor.Data.GetId((ITypeDefOrRef)expr.Operand);
			IRVariable retVar = tr.Context.AllocateVRegister(expr.Type.Value);
			int ecallId = tr.VM.Runtime.VMCall.SIZEOF;
			tr.Instructions.Add(new IRInstruction(IROpCode.VCALL, IRConstant.FromI4(ecallId), IRConstant.FromI4(typeId)));
			tr.Instructions.Add(new IRInstruction(IROpCode.POP, retVar));
			return retVar;
		}
	}
}
