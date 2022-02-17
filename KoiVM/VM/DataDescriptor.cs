using System;
using System.Collections.Generic;
using dnlib.DotNet;

namespace KoiVM.VM
{
	public class DataDescriptor
	{
		internal class FuncSigDesc
		{
			public readonly ITypeDefOrRef DeclaringType;

			public readonly FuncSig FuncSig;

			public readonly uint Id;

			public readonly MethodDef Method;

			public readonly MethodSig Signature;

			public FuncSigDesc(uint id, MethodDef method)
			{
				Id = id;
				Method = method;
				DeclaringType = method.DeclaringType;
				Signature = method.MethodSig;
				FuncSig = new FuncSig();
			}

			public FuncSigDesc(uint id, ITypeDefOrRef declType, MethodSig sig)
			{
				Id = id;
				Method = null;
				DeclaringType = declType;
				Signature = sig;
				FuncSig = new FuncSig();
			}
		}

		private readonly Dictionary<MethodDef, uint> exportMap = new Dictionary<MethodDef, uint>();

		private readonly Dictionary<MethodDef, VMMethodInfo> methodInfos = new Dictionary<MethodDef, VMMethodInfo>();

		private uint nextRefId;

		private uint nextSigId;

		private uint nextStrId;

		private readonly Random random;

		internal Dictionary<IMemberRef, uint> refMap = new Dictionary<IMemberRef, uint>();

		private readonly Dictionary<MethodSig, uint> sigMap = new Dictionary<MethodSig, uint>(SignatureEqualityComparer.Instance);

		internal List<FuncSigDesc> sigs = new List<FuncSigDesc>();

		internal Dictionary<string, uint> strMap = new Dictionary<string, uint>(StringComparer.Ordinal);

		public DataDescriptor(Random random)
		{
			strMap[""] = 1u;
			nextStrId = 2u;
			nextRefId = 1u;
			nextSigId = 1u;
			this.random = random;
		}

		public uint GetId(IMemberRef memberRef)
		{
			if (!refMap.TryGetValue(memberRef, out var ret))
			{
				ret = (refMap[memberRef] = nextRefId++);
			}
			return ret;
		}

		public void ReplaceReference(IMemberRef old, IMemberRef @new)
		{
			if (refMap.TryGetValue(old, out var id))
			{
				refMap.Remove(old);
				refMap[@new] = id;
			}
		}

		public uint GetId(string str)
		{
			if (!strMap.TryGetValue(str, out var ret))
			{
				ret = (strMap[str] = nextStrId++);
			}
			return ret;
		}

		public uint GetId(ITypeDefOrRef declType, MethodSig methodSig)
		{
			if (!sigMap.TryGetValue(methodSig, out var ret))
			{
				uint id = nextSigId++;
				ret = (sigMap[methodSig] = id);
				sigs.Add(new FuncSigDesc(id, declType, methodSig));
			}
			return ret;
		}

		public uint GetExportId(MethodDef method)
		{
			if (!exportMap.TryGetValue(method, out var ret))
			{
				uint id = nextSigId++;
				ret = (exportMap[method] = id);
				sigs.Add(new FuncSigDesc(id, method));
			}
			return ret;
		}

		public VMMethodInfo LookupInfo(MethodDef method)
		{
			if (!methodInfos.TryGetValue(method, out var ret))
			{
				int i = random.Next();
				ret = new VMMethodInfo
				{
					EntryKey = (byte)i,
					ExitKey = (byte)(i >> 8)
				};
				methodInfos[method] = ret;
			}
			return ret;
		}

		public void SetInfo(MethodDef method, VMMethodInfo info)
		{
			methodInfos[method] = info;
		}
	}
}
