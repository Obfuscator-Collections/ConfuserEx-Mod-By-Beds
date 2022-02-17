using KoiVM.RT;

namespace KoiVM.AST.IL
{
	public class ILDataTarget : IILOperand, IHasOffset
	{
		public BinaryChunk Target { get; set; }

		public string Name { get; set; }

		public uint Offset => Target.Offset;

		public ILDataTarget(BinaryChunk target)
		{
			Target = target;
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
