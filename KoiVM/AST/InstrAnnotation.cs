namespace KoiVM.AST
{
	public class InstrAnnotation
	{
		public static readonly InstrAnnotation JUMP = new InstrAnnotation("JUMP");

		public string Name { get; }

		public InstrAnnotation(string name)
		{
			Name = name;
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
