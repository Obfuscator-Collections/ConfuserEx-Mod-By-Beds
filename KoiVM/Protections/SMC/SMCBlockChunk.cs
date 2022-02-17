using dnlib.DotNet;
using KoiVM.RT;

namespace KoiVM.Protections.SMC
{
	internal class SMCBlockChunk : BasicBlockChunk, IKoiChunk
	{
		uint IKoiChunk.Length => base.Length + 1;

		public SMCBlockChunk(VMRuntime rt, MethodDef method, SMCBlock block)
			: base(rt, method, block)
		{
			block.CounterOperand.Value = base.Length + 1;
		}

		void IKoiChunk.OnOffsetComputed(uint offset)
		{
			OnOffsetComputed(offset + 1);
		}

		byte[] IKoiChunk.GetData()
		{
			byte[] data = GetData();
			byte[] newData = new byte[data.Length + 1];
			byte key = ((SMCBlock)base.Block).Key;
			for (int i = 0; i < data.Length; i++)
			{
				newData[i + 1] = (byte)(data[i] ^ key);
			}
			newData[0] = key;
			return newData;
		}
	}
}
