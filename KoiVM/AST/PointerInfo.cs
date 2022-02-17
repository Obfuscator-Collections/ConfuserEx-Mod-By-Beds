using dnlib.DotNet;

namespace KoiVM.AST
{
	public class PointerInfo : InstrAnnotation
	{
		public ITypeDefOrRef PointerType { get; set; }

		public PointerInfo(string name, ITypeDefOrRef ptrType)
			: base(name)
		{
			PointerType = ptrType;
		}
	}
}
