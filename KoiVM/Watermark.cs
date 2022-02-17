using System.Reflection;

namespace KoiVM
{
	[Obfuscation(Exclude = false, Feature = "+koi;-ref proxy")]
	internal static class Watermark
	{
		internal static byte[] GenerateWatermark(uint rand)
		{
			uint id = 65536u;
			uint a = id * 2492804249u;
			uint b = id * 3131742247u;
			uint c = id * 1865781987;
			uint d = a + b + c;
			return new byte[16]
			{
				(byte)(a >> 24),
				(byte)(a >> 16),
				(byte)(a >> 8),
				(byte)a,
				(byte)(b >> 24),
				(byte)(b >> 16),
				(byte)(b >> 8),
				(byte)b,
				(byte)(c >> 24),
				(byte)(c >> 16),
				(byte)(c >> 8),
				(byte)c,
				(byte)(d >> 24),
				(byte)(d >> 16),
				(byte)(d >> 8),
				(byte)d
			};
		}
	}
}
