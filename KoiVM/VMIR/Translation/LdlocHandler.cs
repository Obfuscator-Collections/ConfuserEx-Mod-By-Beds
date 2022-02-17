using dnlib.DotNet;
using dnlib.DotNet.Emit;
using KoiVM.AST.ILAST;
using KoiVM.AST.IR;

namespace KoiVM.VMIR.Translation
{
	public class LdlocHandler : ITranslationHandler
	{
		public Code ILCode => Code.Ldloc;

		public IIROperand Translate(ILASTExpression expr, IRTranslator tr)
		{
			IRVariable local = tr.Context.ResolveLocal((Local)expr.Operand);
			IRVariable ret = tr.Context.AllocateVRegister(local.Type);
			tr.Instructions.Add(new IRInstruction(IROpCode.MOV, ret, local));
			if (local.RawType.ElementType == ElementType.I1 || local.RawType.ElementType == ElementType.I2)
			{
				ret.RawType = local.RawType;
				IRVariable r = tr.Context.AllocateVRegister(local.Type);
				tr.Instructions.Add(new IRInstruction(IROpCode.SX, r, ret));
				ret = r;
			}
			return ret;
		}
	}
}
