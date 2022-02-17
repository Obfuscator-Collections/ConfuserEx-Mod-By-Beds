using System;
using System.Collections.Generic;
using dnlib.DotNet;
using KoiVM.AST.IL;
using KoiVM.CFG;
using KoiVM.RT;
using KoiVM.VMIL.Transforms;

namespace KoiVM.VMIL
{
	public class ILPostTransformer
	{
		private IPostTransform[] pipeline;

		public VMRuntime Runtime { get; }

		public MethodDef Method { get; }

		public ScopeBlock RootScope { get; }

		internal Dictionary<object, object> Annotations { get; }

		internal ILBlock Block { get; private set; }

		internal ILInstrList Instructions => Block.Content;

		public ILPostTransformer(MethodDef method, ScopeBlock rootScope, VMRuntime runtime)
		{
			RootScope = rootScope;
			Method = method;
			Runtime = runtime;
			Annotations = new Dictionary<object, object>();
			pipeline = InitPipeline();
		}

		private IPostTransform[] InitPipeline()
		{
			return new IPostTransform[3]
			{
				new SaveRegistersTransform(),
				new FixMethodRefTransform(),
				new BlockKeyTransform()
			};
		}

		public void Transform()
		{
			if (pipeline == null)
			{
				throw new InvalidOperationException("Transformer already used.");
			}
			IPostTransform[] array = pipeline;
			foreach (IPostTransform handler in array)
			{
				handler.Initialize(this);
				RootScope.ProcessBasicBlocks(delegate(BasicBlock<ILInstrList> block)
				{
					Block = (ILBlock)block;
					handler.Transform(this);
				});
			}
			pipeline = null;
		}
	}
}
