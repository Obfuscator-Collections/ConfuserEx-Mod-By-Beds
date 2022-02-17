#define DEBUG
using System.Diagnostics;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using KoiVM.AST.ILAST;
using KoiVM.AST.IR;

namespace KoiVM.VMIR.Translation
{
	public class UnboxAnyHandler : ITranslationHandler
	{
		public Code ILCode => Code.Unbox_Any;

		public IIROperand Translate(ILASTExpression expr, IRTranslator tr)
		{
			Debug.Assert(expr.Arguments.Length == 1);
			IIROperand value = tr.Translate(expr.Arguments[0]);
			TypeSig targetType = ((ITypeDefOrRef)expr.Operand).ToTypeSig();
			if (!targetType.GetElementType().IsPrimitive() && targetType.ElementType != ElementType.Object && !targetType.ToTypeDefOrRef().ResolveTypeDefThrow().IsEnum)
			{
				return value;
			}
			IRVariable retVar = tr.Context.AllocateVRegister(expr.Type.Value);
			int typeId = (int)tr.VM.Data.GetId((ITypeDefOrRef)expr.Operand);
			int ecallId = tr.VM.Runtime.VMCall.UNBOX;
			tr.Instructions.Add(new IRInstruction(IROpCode.PUSH, value));
			tr.Instructions.Add(new IRInstruction(IROpCode.VCALL, IRConstant.FromI4(ecallId), IRConstant.FromI4(typeId)));
			tr.Instructions.Add(new IRInstruction(IROpCode.POP, retVar));
			return retVar;
		}
	}
}
