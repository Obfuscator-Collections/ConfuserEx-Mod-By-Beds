#define DEBUG
using System.Diagnostics;
using KoiVM.AST;
using KoiVM.AST.IL;
using KoiVM.AST.IR;
using KoiVM.VMIR;

namespace KoiVM.VMIL.Translation
{
	public class SwtHandler : ITranslationHandler
	{
		public IROpCode IRCode => IROpCode.SWT;

		public void Translate(IRInstruction instr, ILTranslator tr)
		{
			tr.PushOperand(instr.Operand2);
			tr.PushOperand(instr.Operand1);
			ILInstruction lastInstr = tr.Instructions[tr.Instructions.Count - 1];
			Debug.Assert(lastInstr.OpCode == ILOpCode.PUSHI_DWORD && lastInstr.Operand is ILJumpTable);
			ILInstruction switchInstr = new ILInstruction(ILOpCode.SWT)
			{
				Annotation = InstrAnnotation.JUMP
			};
			tr.Instructions.Add(switchInstr);
			ILJumpTable jmpTable = (ILJumpTable)lastInstr.Operand;
			jmpTable.Chunk.runtime = tr.Runtime;
			jmpTable.RelativeBase = switchInstr;
			tr.Runtime.AddChunk(jmpTable.Chunk);
		}
	}
}
