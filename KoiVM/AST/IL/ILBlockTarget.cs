using KoiVM.CFG;

namespace KoiVM.AST.IL
{
	public class ILBlockTarget : IILOperand, IHasOffset
	{
		public IBasicBlock Target { get; set; }

		public uint Offset => ((ILBlock)Target).Content[0].Offset;

		public ILBlockTarget(IBasicBlock target)
		{
			Target = target;
		}

		public override string ToString()
		{
			return $"Block_{Target.Id:x2}";
		}
	}
}
