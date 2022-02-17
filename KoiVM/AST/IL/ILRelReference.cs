using KoiVM.RT;

namespace KoiVM.AST.IL
{
	public class ILRelReference : IILOperand
	{
		public IHasOffset Target { get; set; }

		public IHasOffset Base { get; set; }

		public ILRelReference(IHasOffset target, IHasOffset relBase)
		{
			Target = target;
			Base = relBase;
		}

		public virtual uint Resolve(VMRuntime runtime)
		{
			uint relBase = Base.Offset;
			if (Base is ILInstruction)
			{
				relBase += runtime.serializer.ComputeLength((ILInstruction)Base);
			}
			return Target.Offset - relBase;
		}

		public override string ToString()
		{
			return $"[{Base.Offset:x8}:{Target.Offset:x8}]";
		}
	}
}
