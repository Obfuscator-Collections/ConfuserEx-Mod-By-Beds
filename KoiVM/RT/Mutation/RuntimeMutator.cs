using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Writer;
using KoiVM.VM;

namespace KoiVM.RT.Mutation
{
	internal class RuntimeMutator : IModuleWriterListener
	{
		internal RTConstants constants;

		private RuntimeHelpers helpers;

		private readonly MethodPatcher methodPatcher;

		private readonly VMRuntime rt;

		private MetaData rtMD;

		private ModuleWriterBase rtWriter;

		public ModuleDef RTModule { get; set; }

		public byte[] RuntimeLib { get; private set; }

		public byte[] RuntimeSym { get; private set; }

		public event EventHandler<RequestKoiEventArgs> RequestKoi;

		public RuntimeMutator(ModuleDef module, VMRuntime rt)
		{
			RTModule = module;
			this.rt = rt;
			methodPatcher = new MethodPatcher(module);
			constants = new RTConstants();
			helpers = new RuntimeHelpers(constants, rt, module);
			constants.InjectConstants(module, rt.Descriptor, helpers);
			helpers.AddHelpers();
		}

		void IModuleWriterListener.OnWriterEvent(ModuleWriterBase writer, ModuleWriterEvent evt)
		{
			rtWriter = writer;
			rtMD = writer.MetaData;
			if (evt == ModuleWriterEvent.MDEndCreateTables)
			{
				MutateMetadata();
				RequestKoiEventArgs request = new RequestKoiEventArgs();
				this.RequestKoi(this, request);
				writer.TheOptions.MetaDataOptions.OtherHeaps.Add(request.Heap);
				rt.ResetData();
			}
		}

		public void InitHelpers()
		{
			helpers = new RuntimeHelpers(constants, rt, RTModule);
			helpers.AddHelpers();
		}

		public void CommitRuntime(ModuleDef targetModule)
		{
			MutateRuntime();
			if (targetModule == null)
			{
				MemoryStream stream = new MemoryStream();
				MemoryStream pdbStream = new MemoryStream();
				ModuleWriterOptions options = new ModuleWriterOptions(RTModule);
				RTModule.Write(stream, options);
				RuntimeLib = stream.ToArray();
				RuntimeSym = new byte[0];
				return;
			}
			List<TypeDef> types = RTModule.Types.Where((TypeDef t) => !t.IsGlobalModuleType).ToList();
			RTModule.Types.Clear();
			foreach (TypeDef type in types)
			{
				targetModule.Types.Add(type);
			}
		}

		public IModuleWriterListener CommitModule(ModuleDef module)
		{
			ImportReferences(module);
			return this;
		}

		public void ReplaceMethodStub(MethodDef method)
		{
			methodPatcher.PatchMethodStub(method, rt.Descriptor.Data.GetExportId(method));
		}

		private void MutateRuntime()
		{
			IVMSettings settings = rt.Descriptor.Settings;
			RuntimePatcher.Patch(RTModule, settings.ExportDbgInfo, settings.DoStackWalk);
			constants.InjectConstants(RTModule, rt.Descriptor, helpers);
			new Renamer(rt.Descriptor.Random.Next()).Process(RTModule);
		}

		private void ImportReferences(ModuleDef module)
		{
			List<KeyValuePair<IMemberRef, uint>> refCopy = rt.Descriptor.Data.refMap.ToList();
			rt.Descriptor.Data.refMap.Clear();
			foreach (KeyValuePair<IMemberRef, uint> mdRef in refCopy)
			{
				object item = ((!(mdRef.Key is ITypeDefOrRef)) ? ((object)((!(mdRef.Key is MemberRef)) ? ((!(mdRef.Key is MethodDef)) ? ((!(mdRef.Key is MethodSpec)) ? ((!(mdRef.Key is FieldDef)) ? mdRef.Key : module.Import((FieldDef)mdRef.Key)) : module.Import((MethodSpec)mdRef.Key)) : module.Import((MethodDef)mdRef.Key)) : module.Import((MemberRef)mdRef.Key))) : ((object)module.Import((ITypeDefOrRef)mdRef.Key)));
				rt.Descriptor.Data.refMap.Add((IMemberRef)item, mdRef.Value);
			}
			foreach (DataDescriptor.FuncSigDesc sig in rt.Descriptor.Data.sigs)
			{
				MethodSig methodSig = sig.Signature;
				FuncSig funcSig = sig.FuncSig;
				if (methodSig.HasThis)
				{
					funcSig.Flags |= rt.Descriptor.Runtime.RTFlags.INSTANCE;
				}
				List<ITypeDefOrRef> paramTypes = new List<ITypeDefOrRef>();
				if (methodSig.HasThis && !methodSig.ExplicitThis)
				{
					IType thisType = ((!sig.DeclaringType.IsValueType) ? module.Import(sig.DeclaringType) : module.Import(new ByRefSig(sig.DeclaringType.ToTypeSig()).ToTypeDefOrRef()));
					paramTypes.Add((ITypeDefOrRef)thisType);
				}
				foreach (TypeSig param in methodSig.Params)
				{
					ITypeDefOrRef paramType = (ITypeDefOrRef)module.Import(param.ToTypeDefOrRef());
					paramTypes.Add(paramType);
				}
				funcSig.ParamSigs = paramTypes.ToArray();
				ITypeDefOrRef retType = (funcSig.RetType = (ITypeDefOrRef)module.Import(methodSig.RetType.ToTypeDefOrRef()));
			}
		}

		private void MutateMetadata()
		{
			foreach (KeyValuePair<IMemberRef, uint> mdRef in rt.Descriptor.Data.refMap)
			{
				mdRef.Key.Rid = rtMD.GetToken(mdRef.Key).Rid;
			}
			foreach (DataDescriptor.FuncSigDesc sig in rt.Descriptor.Data.sigs)
			{
				FuncSig funcSig = sig.FuncSig;
				ITypeDefOrRef[] paramSigs = funcSig.ParamSigs;
				foreach (ITypeDefOrRef paramType in paramSigs)
				{
					paramType.Rid = rtMD.GetToken(paramType).Rid;
				}
				funcSig.RetType.Rid = rtMD.GetToken(funcSig.RetType).Rid;
			}
		}
	}
}
