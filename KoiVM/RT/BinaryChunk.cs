using System;

namespace KoiVM.RT
{
	public class BinaryChunk : IKoiChunk
	{
		public EventHandler<OffsetComputeEventArgs> OffsetComputed;

		public byte[] Data { get; }

		public uint Offset { get; private set; }

		uint IKoiChunk.Length => (uint)Data.Length;

		public BinaryChunk(byte[] data)
		{
			Data = data;
		}

		void IKoiChunk.OnOffsetComputed(uint offset)
		{
			if (OffsetComputed != null)
			{
				OffsetComputed(this, new OffsetComputeEventArgs(offset));
			}
			Offset = offset;
		}

		byte[] IKoiChunk.GetData()
		{
			return Data;
		}
	}
}
