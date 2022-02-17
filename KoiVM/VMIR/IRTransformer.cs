using System;
using System.Collections.Generic;
using KoiVM.AST.IR;
using KoiVM.CFG;
using KoiVM.RT;
using KoiVM.VM;
using KoiVM.VMIR.Transforms;

namespace KoiVM.VMIR
{
	public class IRTransformer
	{
		private ITransform[] pipeline;

		public IRContext Context { get; }

		public VMRuntime Runtime { get; }

		public VMDescriptor VM => Runtime.Descriptor;

		public ScopeBlock RootScope { get; }

		internal Dictionary<object, object> Annotations { get; }

		internal BasicBlock<IRInstrList> Block { get; private set; }

		internal IRInstrList Instructions => Block.Content;

		public IRTransformer(ScopeBlock rootScope, IRContext ctx, VMRuntime runtime)
		{
			RootScope = rootScope;
			Context = ctx;
			Runtime = runtime;
			Annotations = new Dictionary<object, object>();
			InitPipeline();
		}

		private void InitPipeline()
		{
			pipeline = new ITransform[12]
			{
				Context.IsRuntime ? null : new GuardBlockTransform(),
				Context.IsRuntime ? null : new EHTransform(),
				new InitLocalTransform(),
				new ConstantTypePromotionTransform(),
				new GetSetFlagTransform(),
				new LogicTransform(),
				new InvokeTransform(),
				new MetadataTransform(),
				Context.IsRuntime ? null : new RegisterAllocationTransform(),
				Context.IsRuntime ? null : new StackFrameTransform(),
				new LeaTransform(),
				Context.IsRuntime ? null : new MarkReturnRegTransform()
			};
		}

		public void Transform()
		{
			if (pipeline == null)
			{
				throw new InvalidOperationException("Transformer already used.");
			}
			ITransform[] array = pipeline;
			foreach (ITransform handler in array)
			{
				if (handler != null)
				{
					handler.Initialize(this);
					RootScope.ProcessBasicBlocks(delegate(BasicBlock<IRInstrList> block)
					{
						Block = block;
						handler.Transform(this);
					});
				}
			}
			pipeline = null;
		}
	}
}
