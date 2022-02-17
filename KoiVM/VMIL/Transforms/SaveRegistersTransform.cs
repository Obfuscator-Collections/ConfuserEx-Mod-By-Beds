using System.Collections.Generic;
using System.Linq;
using KoiVM.AST;
using KoiVM.AST.IL;
using KoiVM.AST.IR;
using KoiVM.VM;

namespace KoiVM.VMIL.Transforms
{
	public class SaveRegistersTransform : IPostTransform
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
			if (instr.OpCode != ILOpCode.__BEGINCALL && instr.OpCode != ILOpCode.__ENDCALL)
			{
				return;
			}
			InstrCallInfo callInfo = (InstrCallInfo)instr.Annotation;
			if (callInfo.IsECall)
			{
				instrs.RemoveAt(index);
				index--;
				return;
			}
			HashSet<VMRegisters> saving = new HashSet<VMRegisters>(saveRegs);
			IRVariable retVar = (IRVariable)callInfo.ReturnValue;
			if (retVar != null)
			{
				if (callInfo.ReturnSlot == null)
				{
					VMRegisters retReg = callInfo.ReturnRegister.Register;
					saving.Remove(retReg);
					if (retReg != 0)
					{
						saving.Add(VMRegisters.R0);
					}
				}
				else
				{
					saving.Add(VMRegisters.R0);
				}
			}
			else
			{
				saving.Add(VMRegisters.R0);
			}
			if (instr.OpCode == ILOpCode.__BEGINCALL)
			{
				instrs.Replace(index, saving.Select((VMRegisters reg) => new ILInstruction(ILOpCode.PUSHR_OBJECT, ILRegister.LookupRegister(reg), instr)));
			}
			else
			{
				instrs.Replace(index, saving.Select((VMRegisters reg) => new ILInstruction(ILOpCode.POP, ILRegister.LookupRegister(reg), instr)).Reverse());
			}
			index--;
		}
	}
}
