using KoiVM.AST.IL;
using KoiVM.RT;

namespace KoiVM.Protections.SMC
{
	internal class SMCBlockRef : ILRelReference
	{
		public uint Key { get; set; }

		public SMCBlockRef(IHasOffset target, IHasOffset relBase, uint key)
			: base(target, relBase)
		{
			Key = key;
		}

		public override uint Resolve(VMRuntime runtime)
		{
			return base.Resolve(runtime) ^ Key;
		}
	}
}
