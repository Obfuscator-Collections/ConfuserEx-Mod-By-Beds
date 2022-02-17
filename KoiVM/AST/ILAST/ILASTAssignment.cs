namespace KoiVM.AST.ILAST
{
	public class ILASTAssignment : ASTNode, IILASTStatement
	{
		public ILASTVariable Variable { get; set; }

		public ILASTExpression Value { get; set; }

		public override string ToString()
		{
			return $"{Variable} = {Value}";
		}
	}
}
