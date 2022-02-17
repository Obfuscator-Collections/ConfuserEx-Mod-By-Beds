using dnlib.DotNet;

namespace KoiVM.AST.IR
{
	public class IRVariable : ASTVariable, IIROperand
	{
		public IRVariableType VariableType { get; set; }

		public TypeSig RawType { get; set; }

		public int Id { get; set; }

		public object Annotation { get; set; }

		public override string ToString()
		{
			return $"{base.Name}:{base.Type}";
		}
	}
}
