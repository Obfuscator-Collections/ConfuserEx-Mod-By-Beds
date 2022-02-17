using System;
using System.Collections.Generic;
using dnlib.DotNet;
using KoiVM.AST.ILAST;
using KoiVM.CFG;
using KoiVM.ILAST.Transformation;
using KoiVM.RT;
using KoiVM.VM;

namespace KoiVM.ILAST
{
	public class ILASTTransformer
	{
		private ITransformationHandler[] pipeline;

		public MethodDef Method { get; }

		public ScopeBlock RootScope { get; }

		public VMRuntime Runtime { get; }

		public VMDescriptor VM => Runtime.Descriptor;

		internal Dictionary<object, object> Annotations { get; }

		internal BasicBlock<ILASTTree> Block { get; private set; }

		internal ILASTTree Tree => Block.Content;

		public ILASTTransformer(MethodDef method, ScopeBlock rootScope, VMRuntime runtime)
		{
			RootScope = rootScope;
			Method = method;
			Runtime = runtime;
			Annotations = new Dictionary<object, object>();
			InitPipeline();
		}

		private void InitPipeline()
		{
			pipeline = new ITransformationHandler[7]
			{
				new VariableInlining(),
				new StringTransform(),
				new ArrayTransform(),
				new IndirectTransform(),
				new ILASTTypeInference(),
				new NullTransform(),
				new BranchTransform()
			};
		}

		public void Transform()
		{
			if (pipeline == null)
			{
				throw new InvalidOperationException("Transformer already used.");
			}
			ITransformationHandler[] array = pipeline;
			foreach (ITransformationHandler handler in array)
			{
				handler.Initialize(this);
				RootScope.ProcessBasicBlocks(delegate(BasicBlock<ILASTTree> block)
				{
					Block = block;
					handler.Transform(this);
				});
			}
			pipeline = null;
		}
	}
}
