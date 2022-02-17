using System;
using System.Reflection;
using dnlib.DotNet;
using KoiVM.CFG;
using KoiVM.ILAST;
using KoiVM.RT;
using KoiVM.VMIL;
using KoiVM.VMIR;

namespace KoiVM
{
	[Obfuscation(Exclude = false, Feature = "+koi;-ref proxy")]
	public class MethodVirtualizer
	{
		protected VMRuntime Runtime { get; }

		protected MethodDef Method { get; private set; }

		protected ScopeBlock RootScope { get; private set; }

		protected IRContext IRContext { get; private set; }

		protected bool IsExport { get; private set; }

		public MethodVirtualizer(VMRuntime runtime)
		{
			Runtime = runtime;
		}

		public ScopeBlock Run(MethodDef method, bool isExport)
		{
			try
			{
				Method = method;
				IsExport = isExport;
				Init();
				BuildILAST();
				TransformILAST();
				BuildVMIR();
				TransformVMIR();
				BuildVMIL();
				TransformVMIL();
				Deinitialize();
				ScopeBlock scope = RootScope;
				RootScope = null;
				Method = null;
				return scope;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Failed to translate method {method}.", ex);
				ScopeBlock scope2 = RootScope;
				RootScope = null;
				Method = null;
				return scope2;
			}
		}

		protected virtual void Init()
		{
			RootScope = BlockParser.Parse(Method, Method.Body);
			IRContext = new IRContext(Method, Method.Body);
		}

		protected virtual void BuildILAST()
		{
			ILASTBuilder.BuildAST(Method, Method.Body, RootScope);
		}

		protected virtual void TransformILAST()
		{
			ILASTTransformer transformer = new ILASTTransformer(Method, RootScope, Runtime);
			transformer.Transform();
		}

		protected virtual void BuildVMIR()
		{
			IRTranslator translator = new IRTranslator(IRContext, Runtime);
			translator.Translate(RootScope);
		}

		protected virtual void TransformVMIR()
		{
			IRTransformer transformer = new IRTransformer(RootScope, IRContext, Runtime);
			transformer.Transform();
		}

		protected virtual void BuildVMIL()
		{
			ILTranslator translator = new ILTranslator(Runtime);
			translator.Translate(RootScope);
		}

		protected virtual void TransformVMIL()
		{
			ILTransformer transformer = new ILTransformer(Method, RootScope, Runtime);
			transformer.Transform();
		}

		protected virtual void Deinitialize()
		{
			IRContext = null;
			Runtime.AddMethod(Method, RootScope);
			if (IsExport)
			{
				Runtime.ExportMethod(Method);
			}
		}
	}
}
