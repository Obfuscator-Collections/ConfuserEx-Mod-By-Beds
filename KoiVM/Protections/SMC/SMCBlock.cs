using dnlib.DotNet;
using KoiVM.AST;
using KoiVM.AST.IL;
using KoiVM.RT;

namespace KoiVM.Protections.SMC
{
	internal class SMCBlock : ILBlock
	{
		internal static readonly InstrAnnotation CounterInit = new InstrAnnotation("SMC_COUNTER");

		internal static readonly InstrAnnotation EncryptionKey = new InstrAnnotation("SMC_KEY");

		internal static readonly InstrAnnotation AddressPart1 = new InstrAnnotation("SMC_PART1");

		internal static readonly InstrAnnotation AddressPart2 = new InstrAnnotation("SMC_PART2");

		public byte Key { get; set; }

		public ILImmediate CounterOperand { get; set; }

		public SMCBlock(int id, ILInstrList content)
			: base(id, content)
		{
		}

		public override IKoiChunk CreateChunk(VMRuntime rt, MethodDef method)
		{
			return new SMCBlockChunk(rt, method, this);
		}
	}
}
