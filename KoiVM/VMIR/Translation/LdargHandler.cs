using dnlib.DotNet;
using dnlib.DotNet.Emit;
using KoiVM.AST.ILAST;
using KoiVM.AST.IR;

namespace KoiVM.VMIR.Translation
{
	public class LdargHandler : ITranslationHandler
	{
		public Code ILCode => Code.Ldarg;

		public IIROperand Translate(ILASTExpression expr, IRTranslator tr)
		{
			IRVariable param = tr.Context.ResolveParameter((Parameter)expr.Operand);
			IRVariable ret = tr.Context.AllocateVRegister(param.Type);
			tr.Instructions.Add(new IRInstruction(IROpCode.MOV, ret, param));
			if (param.RawType.ElementType == ElementType.I1 || param.RawType.ElementType == ElementType.I2)
			{
				ret.RawType = param.RawType;
				IRVariable r = tr.Context.AllocateVRegister(param.Type);
				tr.Instructions.Add(new IRInstruction(IROpCode.SX, r, ret));
				ret = r;
			}
			return ret;
		}
	}
}
