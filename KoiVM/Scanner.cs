using System;
using System.Collections.Generic;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace KoiVM
{
	public class Scanner
	{
		private readonly HashSet<MethodDef> exclude = new HashSet<MethodDef>();

		private readonly HashSet<MethodDef> export = new HashSet<MethodDef>();

		private readonly HashSet<MethodDef> methods;

		private readonly ModuleDef module;

		private readonly List<Tuple<MethodDef, bool>> results = new List<Tuple<MethodDef, bool>>();

		public Scanner(ModuleDef module)
			: this(module, null)
		{
		}

		public Scanner(ModuleDef module, HashSet<MethodDef> methods)
		{
			this.module = module;
			this.methods = methods;
		}

		public IEnumerable<Tuple<MethodDef, bool>> Scan()
		{
			ScanMethods(FindExclusion);
			ScanMethods(ScanExport);
			ScanMethods(PopulateResult);
			return results;
		}

		private void ScanMethods(Action<MethodDef> scanFunc)
		{
			foreach (TypeDef type in module.GetTypes())
			{
				foreach (MethodDef method in type.Methods)
				{
					scanFunc(method);
				}
			}
		}

		private void FindExclusion(MethodDef method)
		{
			if (!method.HasBody || (methods != null && !methods.Contains(method)))
			{
				exclude.Add(method);
			}
			else
			{
				if (!method.HasGenericParameters)
				{
					return;
				}
				foreach (Instruction instr in method.Body.Instructions)
				{
					IMethod target = instr.Operand as IMethod;
					if (target != null && target.IsMethod && (target = target.ResolveMethodDef()) != null && (methods == null || methods.Contains((MethodDef)target)))
					{
						export.Add((MethodDef)target);
					}
				}
			}
		}

		private void ScanExport(MethodDef method)
		{
			if (!method.HasBody)
			{
				return;
			}
			bool shouldExport = false;
			shouldExport |= method.IsPublic;
			shouldExport |= method.SemanticsAttributes != MethodSemanticsAttributes.None;
			shouldExport |= method.IsConstructor;
			shouldExport |= method.IsVirtual;
			if (shouldExport | (method.Module.EntryPoint == method))
			{
				export.Add(method);
			}
			bool excluded = exclude.Contains(method) || method.DeclaringType.HasGenericParameters;
			foreach (Instruction instr in method.Body.Instructions)
			{
				if (instr.OpCode == OpCodes.Callvirt || (instr.Operand is IMethod && excluded))
				{
					MethodDef target = ((IMethod)instr.Operand).ResolveMethodDef();
					if (target != null && (methods == null || methods.Contains(target)))
					{
						export.Add(target);
					}
				}
			}
		}

		private void PopulateResult(MethodDef method)
		{
			if (!exclude.Contains(method) && !method.DeclaringType.HasGenericParameters)
			{
				results.Add(Tuple.Create(method, export.Contains(method)));
			}
		}
	}
}
