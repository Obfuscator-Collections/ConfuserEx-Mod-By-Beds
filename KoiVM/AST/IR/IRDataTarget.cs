using KoiVM.RT;

namespace KoiVM.AST.IR
{
	public class IRDataTarget : IIROperand
	{
		public BinaryChunk Target { get; set; }

		public string Name { get; set; }

		public ASTType Type => ASTType.Ptr;

		public IRDataTarget(BinaryChunk target)
		{
			Target = target;
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
