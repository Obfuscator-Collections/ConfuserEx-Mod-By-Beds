using dnlib.DotNet;
using KoiVM.AST.IR;

namespace KoiVM.AST
{
	public class InstrCallInfo : InstrAnnotation
	{
		public ITypeDefOrRef ConstrainType { get; set; }

		public IMethod Method { get; set; }

		public IIROperand[] Arguments { get; set; }

		public IIROperand ReturnValue { get; set; }

		public IRRegister ReturnRegister { get; set; }

		public IRPointer ReturnSlot { get; set; }

		public bool IsECall { get; set; }

		public InstrCallInfo(string name)
			: base(name)
		{
		}

		public override string ToString()
		{
			return base.ToString() + " " + Method;
		}
	}
}
