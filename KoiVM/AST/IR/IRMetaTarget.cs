namespace KoiVM.AST.IR
{
	public class IRMetaTarget : IIROperand
	{
		public object MetadataItem { get; set; }

		public bool LateResolve { get; set; }

		public ASTType Type => ASTType.Ptr;

		public IRMetaTarget(object mdItem)
		{
			MetadataItem = mdItem;
		}

		public override string ToString()
		{
			return MetadataItem.ToString();
		}
	}
}
