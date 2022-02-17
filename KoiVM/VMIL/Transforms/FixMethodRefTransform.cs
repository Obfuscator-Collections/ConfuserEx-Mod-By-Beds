using System.Collections.Generic;
using KoiVM.AST.IL;
using KoiVM.VM;

namespace KoiVM.VMIL.Transforms
{
	public class FixMethodRefTransform : IPostTransform
	{
		private HashSet<VMRegisters> saveRegs;

		public void Initialize(ILPostTransformer tr)
		{
			saveRegs = tr.Runtime.Descriptor.Data.LookupInfo(tr.Method).UsedRegister;
		}

		public void Transform(ILPostTransformer tr)
		{
			tr.Instructions.VisitInstrs(VisitInstr, tr);
		}

		private void VisitInstr(ILInstrList instrs, ILInstruction instr, ref int index, ILPostTransformer tr)
		{
			ILRelReference rel = instr.Operand as ILRelReference;
			if (rel != null)
			{
				(rel.Target as ILMethodTarget)?.Resolve(tr.Runtime);
			}
		}
	}
}
