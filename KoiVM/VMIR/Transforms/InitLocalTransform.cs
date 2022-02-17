using System.Collections.Generic;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using KoiVM.AST;
using KoiVM.AST.IR;

namespace KoiVM.VMIR.Transforms
{
	public class InitLocalTransform : ITransform
	{
		private bool done;

		public void Initialize(IRTransformer tr)
		{
		}

		public void Transform(IRTransformer tr)
		{
			if (tr.Context.Method.Body.InitLocals)
			{
				tr.Instructions.VisitInstrs(VisitInstr, tr);
			}
		}

		private void VisitInstr(IRInstrList instrs, IRInstruction instr, ref int index, IRTransformer tr)
		{
			if (instr.OpCode != IROpCode.__ENTRY || done)
			{
				return;
			}
			List<IRInstruction> init = new List<IRInstruction>();
			init.Add(instr);
			foreach (Local local in tr.Context.Method.Body.Variables)
			{
				if (local.Type.IsValueType && !local.Type.IsPrimitive)
				{
					IRVariable adr = tr.Context.AllocateVRegister(ASTType.ByRef);
					init.Add(new IRInstruction(IROpCode.__LEA, adr, tr.Context.ResolveLocal(local)));
					int typeId = (int)tr.VM.Data.GetId(local.Type.RemovePinnedAndModifiers().ToTypeDefOrRef());
					int ecallId = tr.VM.Runtime.VMCall.INITOBJ;
					init.Add(new IRInstruction(IROpCode.PUSH, adr));
					init.Add(new IRInstruction(IROpCode.VCALL, IRConstant.FromI4(ecallId), IRConstant.FromI4(typeId)));
				}
			}
			instrs.Replace(index, init);
			done = true;
		}
	}
}
