using System;

namespace KoiVM.RT
{
	public class OffsetComputeEventArgs : EventArgs
	{
		public uint Offset { get; }

		internal OffsetComputeEventArgs(uint offset)
		{
			Offset = offset;
		}
	}
}
