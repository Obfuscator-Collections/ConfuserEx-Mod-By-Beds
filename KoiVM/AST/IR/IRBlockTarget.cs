using KoiVM.CFG;

namespace KoiVM.AST.IR
{
	public class IRBlockTarget : IIROperand
	{
		public IBasicBlock Target { get; set; }

		public ASTType Type => ASTType.Ptr;

		public IRBlockTarget(IBasicBlock target)
		{
			Target = target;
		}

		public override string ToString()
		{
			return $"Block_{Target.Id:x2}";
		}
	}
}
