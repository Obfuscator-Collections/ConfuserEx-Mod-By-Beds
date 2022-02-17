namespace KoiVM.AST.IR
{
	public class IRPointer : IIROperand
	{
		public IRRegister Register { get; set; }

		public int Offset { get; set; }

		public IRVariable SourceVariable { get; set; }

		public ASTType Type { get; set; }

		public override string ToString()
		{
			string prefix = Type.ToString();
			string offsetStr = "";
			if (Offset > 0)
			{
				offsetStr = $" + {Offset:x}h";
			}
			else if (Offset < 0)
			{
				offsetStr = $" - {-Offset:x}h";
			}
			return $"{prefix}:[{Register}{offsetStr}]";
		}
	}
}
