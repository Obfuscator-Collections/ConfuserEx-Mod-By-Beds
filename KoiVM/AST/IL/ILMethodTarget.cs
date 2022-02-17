using dnlib.DotNet;
using KoiVM.RT;

namespace KoiVM.AST.IL
{
	public class ILMethodTarget : IILOperand, IHasOffset
	{
		private ILBlock methodEntry;

		public MethodDef Target { get; set; }

		public uint Offset => (methodEntry != null) ? methodEntry.Content[0].Offset : 0u;

		public ILMethodTarget(MethodDef target)
		{
			Target = target;
		}

		public void Resolve(VMRuntime runtime)
		{
			runtime.LookupMethod(Target, out methodEntry);
		}

		public override string ToString()
		{
			return Target.ToString();
		}
	}
}
