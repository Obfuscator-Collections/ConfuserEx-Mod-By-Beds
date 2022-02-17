using KoiVM.CFG;
using KoiVM.RT;

namespace KoiVM.AST.IL
{
	public class ILJumpTable : IILOperand, IHasOffset
	{
		public JumpTableChunk Chunk { get; }

		public ILInstruction RelativeBase { get; set; }

		public IBasicBlock[] Targets { get; set; }

		public uint Offset => Chunk.Offset;

		public ILJumpTable(IBasicBlock[] targets)
		{
			Targets = targets;
			Chunk = new JumpTableChunk(this);
		}

		public override string ToString()
		{
			return $"[..{Targets.Length}..]";
		}
	}
}
