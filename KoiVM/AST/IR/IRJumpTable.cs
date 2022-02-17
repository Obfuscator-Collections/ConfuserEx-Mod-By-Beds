using KoiVM.CFG;

namespace KoiVM.AST.IR
{
	public class IRJumpTable : IIROperand
	{
		public IBasicBlock[] Targets { get; set; }

		public ASTType Type => ASTType.Ptr;

		public IRJumpTable(IBasicBlock[] targets)
		{
			Targets = targets;
		}

		public override string ToString()
		{
			return $"[..{Targets.Length}..]";
		}
	}
}
