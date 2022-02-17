using dnlib.DotNet;
using dnlib.DotNet.Emit;
using KoiVM.AST.IL;
using KoiVM.AST.IR;
using KoiVM.CFG;
using KoiVM.VM;
using KoiVM.VMIL;
using KoiVM.VMIR;

namespace KoiVM.RT.Mutation
{
	public class RuntimeHelpers
	{
		private RTConstants constants;

		private MethodDef methodINIT;

		private readonly VMRuntime rt;

		private TypeDef rtHelperType;

		private readonly ModuleDef rtModule;

		public uint INIT { get; private set; }

		public RuntimeHelpers(RTConstants constants, VMRuntime rt, ModuleDef rtModule)
		{
			this.rt = rt;
			this.rtModule = rtModule;
			this.constants = constants;
			rtHelperType = new TypeDefUser("KoiVM.Runtime", "Helpers");
			AllocateHelpers();
		}

		private MethodDef CreateHelperMethod(string name)
		{
			MethodDefUser helper = new MethodDefUser(name, MethodSig.CreateStatic(rtModule.CorLibTypes.Void));
			helper.Body = new CilBody();
			return helper;
		}

		private void AllocateHelpers()
		{
			methodINIT = CreateHelperMethod("INIT");
			INIT = rt.Descriptor.Data.GetExportId(methodINIT);
		}

		public void AddHelpers()
		{
			ScopeBlock scope = new ScopeBlock();
			BasicBlock<IRInstrList> initBlock = new BasicBlock<IRInstrList>(1, new IRInstrList
			{
				new IRInstruction(IROpCode.RET)
			});
			scope.Content.Add(initBlock);
			BasicBlock<IRInstrList> retnBlock = new BasicBlock<IRInstrList>(0, new IRInstrList
			{
				new IRInstruction(IROpCode.VCALL, IRConstant.FromI4(rt.Descriptor.Runtime.VMCall[VMCalls.EXIT]))
			});
			scope.Content.Add(initBlock);
			CompileHelpers(methodINIT, scope);
			VMMethodInfo info = rt.Descriptor.Data.LookupInfo(methodINIT);
			scope.ProcessBasicBlocks(delegate(BasicBlock<ILInstrList> block)
			{
				if (block.Id == 1)
				{
					AddHelper(null, methodINIT, (ILBlock)block);
					VMBlockKey vMBlockKey = info.BlockKeys[block];
					info.EntryKey = vMBlockKey.EntryKey;
					info.ExitKey = vMBlockKey.ExitKey;
				}
				rt.AddBlock(methodINIT, (ILBlock)block);
			});
		}

		private void AddHelper(VMMethodInfo info, MethodDef method, ILBlock block)
		{
			ScopeBlock helperScope = new ScopeBlock();
			block.Id = 0;
			helperScope.Content.Add(block);
			if (info != null)
			{
				VMMethodInfo helperInfo = new VMMethodInfo();
				VMBlockKey keys = info.BlockKeys[block];
				helperInfo.RootScope = helperScope;
				helperInfo.EntryKey = keys.EntryKey;
				helperInfo.ExitKey = keys.ExitKey;
				rt.Descriptor.Data.SetInfo(method, helperInfo);
			}
			rt.AddHelper(method, helperScope, block);
		}

		private void CompileHelpers(MethodDef method, ScopeBlock scope)
		{
			IRContext methodCtx = new IRContext(method, method.Body);
			methodCtx.IsRuntime = true;
			IRTransformer irTransformer = new IRTransformer(scope, methodCtx, rt);
			irTransformer.Transform();
			ILTranslator ilTranslator = new ILTranslator(rt);
			ILTransformer ilTransformer = new ILTransformer(method, scope, rt);
			ilTranslator.Translate(scope);
			ilTransformer.Transform();
			ILPostTransformer postTransformer = new ILPostTransformer(method, scope, rt);
			postTransformer.Transform();
		}
	}
}
