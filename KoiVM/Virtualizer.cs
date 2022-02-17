using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using dnlib.DotNet;
using dnlib.DotNet.Writer;
using KoiVM.CFG;
using KoiVM.RT;
using KoiVM.VMIL;

namespace KoiVM
{
	[Obfuscation(Exclude = false, Feature = "+koi;-ref proxy")]
	public class Virtualizer : IVMSettings
	{
		private readonly bool debug;

		private readonly HashSet<MethodDef> doInstantiation = new HashSet<MethodDef>();

		private readonly GenericInstantiation instantiation = new GenericInstantiation();

		private readonly Dictionary<MethodDef, bool> methodList = new Dictionary<MethodDef, bool>();

		private readonly HashSet<ModuleDef> processed = new HashSet<ModuleDef>();

		private string runtimeName;

		private readonly int seed;

		private MethodVirtualizer vr;

		public ModuleDef RuntimeModule => Runtime.Module;

		public VMRuntime Runtime { get; set; }

		int IVMSettings.Seed => seed;

		bool IVMSettings.IsDebug => debug;

		public bool ExportDbgInfo { get; set; }

		public bool DoStackWalk { get; set; }

		public Virtualizer(int seed, bool debug)
		{
			Runtime = null;
			this.seed = seed;
			this.debug = debug;
			instantiation.ShouldInstantiate += (MethodSpec spec) => doInstantiation.Contains(spec.Method.ResolveMethodDefThrow());
		}

		bool IVMSettings.IsExported(MethodDef method)
		{
			if (!methodList.TryGetValue(method, out var ret))
			{
				return false;
			}
			return ret;
		}

		bool IVMSettings.IsVirtualized(MethodDef method)
		{
			return methodList.ContainsKey(method);
		}

		public void Initialize(ModuleDef runtimeLib)
		{
			Runtime = new VMRuntime(this, runtimeLib);
			runtimeName = runtimeLib.Assembly.Name;
			vr = new MethodVirtualizer(Runtime);
		}

		public void AddModule(ModuleDef module)
		{
			foreach (Tuple<MethodDef, bool> method in new Scanner(module).Scan())
			{
				AddMethod(method.Item1, method.Item2);
			}
		}

		public void AddMethod(MethodDef method, bool isExport)
		{
			if (!method.HasBody || method.HasGenericParameters)
			{
				return;
			}
			methodList.Add(method, isExport);
			if (isExport)
			{
				return;
			}
			TypeSig thisParam = (method.HasThis ? method.Parameters[0].Type : null);
			TypeDef declType = method.DeclaringType;
			declType.Methods.Remove(method);
			if (method.SemanticsAttributes != 0)
			{
				foreach (PropertyDef prop in declType.Properties)
				{
					if (prop.GetMethod == method)
					{
						prop.GetMethod = null;
					}
					if (prop.SetMethod == method)
					{
						prop.SetMethod = null;
					}
				}
				foreach (EventDef evt in declType.Events)
				{
					if (evt.AddMethod == method)
					{
						evt.AddMethod = null;
					}
					if (evt.RemoveMethod == method)
					{
						evt.RemoveMethod = null;
					}
					if (evt.InvokeMethod == method)
					{
						evt.InvokeMethod = null;
					}
				}
			}
			method.DeclaringType2 = declType;
			if (thisParam != null)
			{
				method.Parameters[0].Type = thisParam;
			}
		}

		public IEnumerable<MethodDef> GetMethods()
		{
			return methodList.Keys;
		}

		public void ProcessMethods(ModuleDef module, Action<int, int> progress = null)
		{
			if (processed.Contains(module))
			{
				throw new InvalidOperationException("Module already processed.");
			}
			if (progress == null)
			{
				progress = delegate
				{
				};
			}
			List<MethodDef> targets = methodList.Keys.Where((MethodDef method) => method.Module == module).ToList();
			for (int i = 0; i < targets.Count; i++)
			{
				MethodDef method2 = targets[i];
				instantiation.EnsureInstantiation(method2, delegate(MethodSpec spec, MethodDef instantation)
				{
					if (instantation.Module == module || processed.Contains(instantation.Module))
					{
						targets.Add(instantation);
					}
					methodList[instantation] = false;
				});
				try
				{
					ProcessMethod(method2, methodList[method2]);
				}
				catch (Exception)
				{
					Console.WriteLine("! error on process method : " + method2.FullName);
				}
				progress(i, targets.Count);
			}
			progress(targets.Count, targets.Count);
			processed.Add(module);
		}

		public IModuleWriterListener CommitModule(ModuleDefMD module, Action<int, int> progress = null)
		{
			if (progress == null)
			{
				progress = delegate
				{
				};
			}
			MethodDef[] methods = methodList.Keys.Where((MethodDef method) => method.Module == module).ToArray();
			for (int i = 0; i < methods.Length; i++)
			{
				MethodDef method2 = methods[i];
				PostProcessMethod(method2, methodList[method2]);
				progress(i, methodList.Count);
			}
			progress(methods.Length, methods.Length);
			return Runtime.CommitModule(module);
		}

		public void CommitRuntime(ModuleDef targetModule = null)
		{
			Runtime.CommitRuntime(targetModule);
		}

		private void ProcessMethod(MethodDef method, bool isExport)
		{
			vr.Run(method, isExport);
		}

		private void PostProcessMethod(MethodDef method, bool isExport)
		{
			ScopeBlock scope = Runtime.LookupMethod(method);
			ILPostTransformer ilTransformer = new ILPostTransformer(method, scope, Runtime);
			ilTransformer.Transform();
		}

		public string SaveRuntime(string directory)
		{
			string rtPath = Path.Combine(directory, runtimeName + ".dll");
			File.WriteAllBytes(rtPath, Runtime.RuntimeLibrary);
			if (Runtime.RuntimeSymbols.Length != 0)
			{
				File.WriteAllBytes(Path.ChangeExtension(rtPath, "pdb"), Runtime.RuntimeSymbols);
			}
			return rtPath;
		}
	}
}
