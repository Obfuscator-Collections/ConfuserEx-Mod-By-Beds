using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Confuser.Core.Project;
using Confuser.Core.Project.Patterns;
using dnlib.DotNet;

namespace Confuser.Core
{
	/// <summary>
	/// Obfuscation Attribute Marker
	/// </summary>
	// Token: 0x02000048 RID: 72
	public class ObfAttrMarker : Marker
	{
		// Token: 0x060001B2 RID: 434 RVA: 0x0000DA78 File Offset: 0x0000BC78
		private static IEnumerable<ObfAttrMarker.ObfuscationAttributeInfo> ReadObfuscationAttributes(IHasCustomAttribute item)
		{
			List<ObfAttrMarker.ObfuscationAttributeInfo> ret = new List<ObfAttrMarker.ObfuscationAttributeInfo>();
			for (int i = item.CustomAttributes.Count - 1; i >= 0; i--)
			{
				CustomAttribute ca = item.CustomAttributes[i];
				if (!(ca.TypeFullName != "System.Reflection.ObfuscationAttribute"))
				{
					ObfAttrMarker.ObfuscationAttributeInfo info = default(ObfAttrMarker.ObfuscationAttributeInfo);
					bool strip = true;
					foreach (CANamedArgument prop in ca.Properties)
					{
						string a;
						if ((a = prop.Name) != null)
						{
							if (a == "ApplyToMembers")
							{
								info.ApplyToMembers = new bool?((bool)prop.Value);
								continue;
							}
							if (a == "Exclude")
							{
								info.Exclude = new bool?((bool)prop.Value);
								continue;
							}
							if (a == "StripAfterObfuscation")
							{
								strip = (bool)prop.Value;
								continue;
							}
							if (a == "Feature")
							{
								string feature = (UTF8String)prop.Value;
								int sepIndex = feature.IndexOf(':');
								if (sepIndex == -1)
								{
									info.FeatureName = "";
									info.FeatureValue = feature;
									continue;
								}
								info.FeatureName = feature.Substring(0, sepIndex);
								info.FeatureValue = feature.Substring(sepIndex + 1);
								continue;
							}
						}
						throw new NotSupportedException("Unsupported property: " + prop.Name);
					}
					if (strip)
					{
						item.CustomAttributes.RemoveAt(i);
					}
					ret.Add(info);
				}
			}
			ret.Reverse();
			return ret;
		}

		// Token: 0x060001B3 RID: 435 RVA: 0x0000DFE4 File Offset: 0x0000C1E4
		private IEnumerable<ObfAttrMarker.ProtectionSettingsInfo> ProcessAttributes(IEnumerable<ObfAttrMarker.ObfuscationAttributeInfo> attrs)
		{
			bool flag = false;
			foreach (ObfAttrMarker.ObfuscationAttributeInfo current in attrs)
			{
				ObfAttrMarker.ProtectionSettingsInfo protectionSettingsInfo = default(ObfAttrMarker.ProtectionSettingsInfo);
				protectionSettingsInfo.Exclude = (current.Exclude ?? true);
				protectionSettingsInfo.ApplyToMember = (current.ApplyToMembers ?? true);
				protectionSettingsInfo.Settings = current.FeatureValue;
				bool flag2 = true;
				try
				{
					new ObfAttrParser(this.protections).ParseProtectionString(null, protectionSettingsInfo.Settings);
				}
				catch
				{
					flag2 = false;
				}
				if (!flag2)
				{
					this.context.Logger.WarnFormat("Ignoring rule '{0}'.", new object[]
					{
						protectionSettingsInfo.Settings
					});
				}
				else
				{
					if (!string.IsNullOrEmpty(current.FeatureName))
					{
						throw new ArgumentException("Feature name must not be set.");
					}
					if (protectionSettingsInfo.Exclude && (!string.IsNullOrEmpty(current.FeatureName) || !string.IsNullOrEmpty(current.FeatureValue)))
					{
						throw new ArgumentException("Feature property cannot be set when Exclude is true.");
					}
					yield return protectionSettingsInfo;
					flag = true;
				}
			}
			if (!flag)
			{
				ObfAttrMarker.ProtectionSettingsInfo protectionSettingsInfo = default(ObfAttrMarker.ProtectionSettingsInfo);
				protectionSettingsInfo.Exclude = false;
				protectionSettingsInfo.ApplyToMember = false;
				protectionSettingsInfo.Settings = "";
				yield return protectionSettingsInfo;
			}
			yield break;
		}

		// Token: 0x060001B4 RID: 436 RVA: 0x0000E008 File Offset: 0x0000C208
		private void ApplySettings(IDnlibDef def, Dictionary<Rule, PatternExpression> rules, IEnumerable<ObfAttrMarker.ProtectionSettingsInfo> infos, ProtectionSettings settings = null)
		{
			if (settings == null)
			{
				settings = new ProtectionSettings();
			}
			else
			{
				settings = new ProtectionSettings(settings);
			}
			base.ApplyRules(this.context, def, rules, settings);
			settings = ProtectionParameters.GetParameters(this.context, def);
			ObfAttrMarker.ProtectionSettingsInfo? last = null;
			ObfAttrParser parser = new ObfAttrParser(this.protections);
			foreach (ObfAttrMarker.ProtectionSettingsInfo info in infos)
			{
				if (info.Exclude)
				{
					if (info.ApplyToMember)
					{
						settings.Clear();
					}
				}
				else
				{
					last = new ObfAttrMarker.ProtectionSettingsInfo?(info);
					if (info.ApplyToMember && !string.IsNullOrEmpty(info.Settings))
					{
						parser.ParseProtectionString(settings, info.Settings);
					}
				}
			}
			if (last.HasValue && !last.Value.ApplyToMember && !string.IsNullOrEmpty(last.Value.Settings))
			{
				parser.ParseProtectionString(settings, last.Value.Settings);
			}
		}

		/// <inheritdoc />
		// Token: 0x060001B5 RID: 437 RVA: 0x0000E11C File Offset: 0x0000C31C
		protected internal override void MarkMember(IDnlibDef member, ConfuserContext context)
		{
			ModuleDef module = ((IMemberRef)member).Module;
			ProtectionParameters.SetParameters(context, member, ProtectionParameters.GetParameters(context, module));
		}

		/// <inheritdoc />
		// Token: 0x060001B6 RID: 438 RVA: 0x0000E14C File Offset: 0x0000C34C
		protected internal override MarkerResult MarkProject(ConfuserProject proj, ConfuserContext context)
		{
			this.crossModuleAttrs = new Dictionary<string, Dictionary<Regex, List<ObfAttrMarker.ObfuscationAttributeInfo>>>();
			this.context = context;
			this.project = proj;
			this.extModules = new List<byte[]>();
			if (proj.Packer != null)
			{
				if (!this.packers.ContainsKey(proj.Packer.Id))
				{
					context.Logger.ErrorFormat("Cannot find packer with ID '{0}'.", new object[]
					{
						proj.Packer.Id
					});
					throw new ConfuserException(null);
				}
				this.packer = this.packers[proj.Packer.Id];
				this.packerParams = new Dictionary<string, string>(proj.Packer, StringComparer.OrdinalIgnoreCase);
			}
			List<Tuple<ProjectModule, ModuleDefMD>> modules = new List<Tuple<ProjectModule, ModuleDefMD>>();
			foreach (ProjectModule module3 in proj)
			{
				if (module3.IsExternal)
				{
					this.extModules.Add(module3.LoadRaw(proj.BaseDirectory));
				}
				else
				{
					ModuleDefMD modDef = module3.Resolve(proj.BaseDirectory, context.Resolver.DefaultModuleContext);
					context.CheckCancellation();
					context.Resolver.AddToCache(modDef);
					modules.Add(Tuple.Create<ProjectModule, ModuleDefMD>(module3, modDef));
				}
			}
			foreach (Tuple<ProjectModule, ModuleDefMD> module2 in modules)
			{
				context.Logger.InfoFormat("Loading '{0}'...", new object[]
				{
					module2.Item1.Path
				});
				Dictionary<Rule, PatternExpression> rules = base.ParseRules(proj, module2.Item1, context);
				this.MarkModule(module2.Item1, module2.Item2, rules, module2 == modules[0]);
				context.Annotations.Set<Dictionary<Rule, PatternExpression>>(module2.Item2, Marker.RulesKey, rules);
				if (this.packer != null)
				{
					ProtectionParameters.GetParameters(context, module2.Item2)[this.packer] = this.packerParams;
				}
			}
			if (proj.Debug && proj.Packer != null)
			{
				context.Logger.Warn("Generated Debug symbols might not be usable with packers!");
			}
			return new MarkerResult((from module in modules
			select module.Item2).ToList<ModuleDefMD>(), this.packer, this.extModules);
		}

		// Token: 0x060001B7 RID: 439 RVA: 0x0000E3C4 File Offset: 0x0000C5C4
		private void MarkModule(ProjectModule projModule, ModuleDefMD module, Dictionary<Rule, PatternExpression> rules, bool isMain)
		{
			List<ObfAttrMarker.ObfuscationAttributeInfo> settingAttrs = new List<ObfAttrMarker.ObfuscationAttributeInfo>();
			string snKeyPath = projModule.SNKeyPath;
			string snKeyPass = projModule.SNKeyPassword;
			Dictionary<Regex, List<ObfAttrMarker.ObfuscationAttributeInfo>> namespaceAttrs;
			if (!this.crossModuleAttrs.TryGetValue(module.Name, out namespaceAttrs))
			{
				namespaceAttrs = new Dictionary<Regex, List<ObfAttrMarker.ObfuscationAttributeInfo>>();
			}
			foreach (ObfAttrMarker.ObfuscationAttributeInfo attr in ObfAttrMarker.ReadObfuscationAttributes(module.Assembly))
			{
				if (string.IsNullOrEmpty(attr.FeatureName))
				{
					settingAttrs.Add(attr);
				}
				else if (attr.FeatureName.Equals("generate debug symbol", StringComparison.OrdinalIgnoreCase))
				{
					if (!isMain)
					{
						throw new ArgumentException("Only main module can set 'generate debug symbol'.");
					}
					this.project.Debug = bool.Parse(attr.FeatureValue);
				}
				else if (attr.FeatureName.Equals("random seed", StringComparison.OrdinalIgnoreCase))
				{
					if (!isMain)
					{
						throw new ArgumentException("Only main module can set 'random seed'.");
					}
					this.project.Seed = attr.FeatureValue;
				}
				else if (attr.FeatureName.Equals("strong name key", StringComparison.OrdinalIgnoreCase))
				{
					snKeyPath = Path.Combine(this.project.BaseDirectory, attr.FeatureValue);
				}
				else if (attr.FeatureName.Equals("strong name key password", StringComparison.OrdinalIgnoreCase))
				{
					snKeyPass = attr.FeatureValue;
				}
				else if (attr.FeatureName.Equals("packer", StringComparison.OrdinalIgnoreCase))
				{
					if (!isMain)
					{
						throw new ArgumentException("Only main module can set 'packer'.");
					}
					new ObfAttrParser(this.packers).ParsePackerString(attr.FeatureValue, out this.packer, out this.packerParams);
				}
				else if (attr.FeatureName.Equals("external module", StringComparison.OrdinalIgnoreCase))
				{
					if (!isMain)
					{
						throw new ArgumentException("Only main module can add external modules.");
					}
					byte[] rawModule = new ProjectModule
					{
						Path = attr.FeatureValue
					}.LoadRaw(this.project.BaseDirectory);
					this.extModules.Add(rawModule);
				}
				else
				{
					Match match = ObfAttrMarker.NSInModulePattern.Match(attr.FeatureName);
					if (match.Success)
					{
						if (!isMain)
						{
							throw new ArgumentException("Only main module can set cross module obfuscation.");
						}
						Regex ns = ObfAttrMarker.TranslateNamespaceRegex(match.Groups[1].Value);
						string targetModule = match.Groups[2].Value;
						ObfAttrMarker.ObfuscationAttributeInfo x = attr;
						x.FeatureName = "";
						Dictionary<Regex, List<ObfAttrMarker.ObfuscationAttributeInfo>> targetModuleAttrs;
						if (!this.crossModuleAttrs.TryGetValue(targetModule, out targetModuleAttrs))
						{
							targetModuleAttrs = new Dictionary<Regex, List<ObfAttrMarker.ObfuscationAttributeInfo>>();
							this.crossModuleAttrs[targetModule] = targetModuleAttrs;
						}
						targetModuleAttrs.AddListEntry(ns, x);
					}
					else
					{
						match = ObfAttrMarker.NSPattern.Match(attr.FeatureName);
						if (match.Success)
						{
							Regex ns2 = ObfAttrMarker.TranslateNamespaceRegex(match.Groups[1].Value);
							ObfAttrMarker.ObfuscationAttributeInfo x2 = attr;
							x2.FeatureName = "";
							namespaceAttrs.AddListEntry(ns2, x2);
						}
					}
				}
			}
			if (this.project.Debug)
			{
				module.LoadPdb();
			}
			this.ProcessModule(module, rules, snKeyPath, snKeyPass, settingAttrs, namespaceAttrs);
		}

		// Token: 0x060001B8 RID: 440 RVA: 0x0000E6FC File Offset: 0x0000C8FC
		private static Regex TranslateNamespaceRegex(string ns)
		{
			if (ns == "*")
			{
				return new Regex(".*");
			}
			if (ns.Length >= 2 && ns[0] == '*' && ns[ns.Length - 1] == '*')
			{
				return new Regex(Regex.Escape(ns.Substring(1, ns.Length - 2)));
			}
			if (ns.Length >= 1 && ns[0] == '*')
			{
				return new Regex(Regex.Escape(ns.Substring(1)) + "$");
			}
			if (ns.Length >= 1 && ns[ns.Length - 1] == '*')
			{
				return new Regex(Regex.Escape("^" + ns.Substring(0, ns.Length - 1)));
			}
			return new Regex("^" + Regex.Escape(ns) + "$");
		}

		// Token: 0x060001B9 RID: 441 RVA: 0x0000E7EC File Offset: 0x0000C9EC
		private static ObfAttrMarker.ProtectionSettingsStack MatchNamespace(Dictionary<Regex, ObfAttrMarker.ProtectionSettingsStack> attrs, string ns)
		{
			foreach (KeyValuePair<Regex, ObfAttrMarker.ProtectionSettingsStack> nsStack in attrs)
			{
				if (nsStack.Key.IsMatch(ns))
				{
					return nsStack.Value;
				}
			}
			return null;
		}

		// Token: 0x060001BA RID: 442 RVA: 0x0000E898 File Offset: 0x0000CA98
		private void ProcessModule(ModuleDefMD module, Dictionary<Rule, PatternExpression> rules, string snKeyPath, string snKeyPass, List<ObfAttrMarker.ObfuscationAttributeInfo> settingAttrs, Dictionary<Regex, List<ObfAttrMarker.ObfuscationAttributeInfo>> namespaceAttrs)
		{
			this.context.Annotations.Set<StrongNameKey>(module, Marker.SNKey, Marker.LoadSNKey(this.context, (snKeyPath == null) ? null : Path.Combine(this.project.BaseDirectory, snKeyPath), snKeyPass));
			ObfAttrMarker.ProtectionSettingsStack moduleStack = new ObfAttrMarker.ProtectionSettingsStack();
			moduleStack.Push(this.ProcessAttributes(settingAttrs));
			this.ApplySettings(module, rules, moduleStack.GetInfos(), null);
			Dictionary<Regex, ObfAttrMarker.ProtectionSettingsStack> nsSettings = namespaceAttrs.ToDictionary((KeyValuePair<Regex, List<ObfAttrMarker.ObfuscationAttributeInfo>> kvp) => kvp.Key, delegate(KeyValuePair<Regex, List<ObfAttrMarker.ObfuscationAttributeInfo>> kvp)
			{
				ObfAttrMarker.ProtectionSettingsStack nsStack = new ObfAttrMarker.ProtectionSettingsStack(moduleStack);
				nsStack.Push(this.ProcessAttributes(kvp.Value));
				return nsStack;
			});
			foreach (TypeDef type in module.Types)
			{
				ObfAttrMarker.ProtectionSettingsStack typeStack = ObfAttrMarker.MatchNamespace(nsSettings, type.Namespace) ?? moduleStack;
				typeStack.Push(this.ProcessAttributes(ObfAttrMarker.ReadObfuscationAttributes(type)));
				this.ApplySettings(type, rules, typeStack.GetInfos(), null);
				this.ProcessTypeMembers(type, rules, typeStack);
				typeStack.Pop();
			}
		}

		// Token: 0x060001BB RID: 443 RVA: 0x0000E9DC File Offset: 0x0000CBDC
		private void ProcessTypeMembers(TypeDef type, Dictionary<Rule, PatternExpression> rules, ObfAttrMarker.ProtectionSettingsStack stack)
		{
			foreach (TypeDef nestedType in type.NestedTypes)
			{
				stack.Push(this.ProcessAttributes(ObfAttrMarker.ReadObfuscationAttributes(nestedType)));
				this.ApplySettings(nestedType, rules, stack.GetInfos(), null);
				this.ProcessTypeMembers(nestedType, rules, stack);
				stack.Pop();
			}
			foreach (PropertyDef prop in type.Properties)
			{
				stack.Push(this.ProcessAttributes(ObfAttrMarker.ReadObfuscationAttributes(prop)));
				this.ApplySettings(prop, rules, stack.GetInfos(), null);
				if (prop.GetMethod != null)
				{
					this.ProcessMember(prop.GetMethod, rules, stack);
				}
				if (prop.SetMethod != null)
				{
					this.ProcessMember(prop.SetMethod, rules, stack);
				}
				foreach (MethodDef i in prop.OtherMethods)
				{
					this.ProcessMember(i, rules, stack);
				}
				stack.Pop();
			}
			foreach (EventDef evt in type.Events)
			{
				stack.Push(this.ProcessAttributes(ObfAttrMarker.ReadObfuscationAttributes(evt)));
				this.ApplySettings(evt, rules, stack.GetInfos(), null);
				if (evt.AddMethod != null)
				{
					this.ProcessMember(evt.AddMethod, rules, stack);
				}
				if (evt.RemoveMethod != null)
				{
					this.ProcessMember(evt.RemoveMethod, rules, stack);
				}
				if (evt.InvokeMethod != null)
				{
					this.ProcessMember(evt.InvokeMethod, rules, stack);
				}
				foreach (MethodDef j in evt.OtherMethods)
				{
					this.ProcessMember(j, rules, stack);
				}
				stack.Pop();
			}
			foreach (MethodDef method in type.Methods)
			{
				if (method.SemanticsAttributes == MethodSemanticsAttributes.None)
				{
					this.ProcessMember(method, rules, stack);
				}
			}
			foreach (FieldDef field in type.Fields)
			{
				this.ProcessMember(field, rules, stack);
			}
		}

		// Token: 0x060001BC RID: 444 RVA: 0x0000ECB0 File Offset: 0x0000CEB0
		private void ProcessMember(IDnlibDef member, Dictionary<Rule, PatternExpression> rules, ObfAttrMarker.ProtectionSettingsStack stack)
		{
			stack.Push(this.ProcessAttributes(ObfAttrMarker.ReadObfuscationAttributes(member)));
			this.ApplySettings(member, rules, stack.GetInfos(), null);
			stack.Pop();
		}

		// Token: 0x04000141 RID: 321
		private static readonly Regex NSPattern = new Regex("namespace '([^']*)'");

		// Token: 0x04000142 RID: 322
		private static readonly Regex NSInModulePattern = new Regex("namespace '([^']*)' in module '([^'])'");

		// Token: 0x04000143 RID: 323
		private Dictionary<string, Dictionary<Regex, List<ObfAttrMarker.ObfuscationAttributeInfo>>> crossModuleAttrs;

		// Token: 0x04000144 RID: 324
		private ConfuserContext context;

		// Token: 0x04000145 RID: 325
		private ConfuserProject project;

		// Token: 0x04000146 RID: 326
		private Packer packer;

		// Token: 0x04000147 RID: 327
		private Dictionary<string, string> packerParams;

		// Token: 0x04000148 RID: 328
		private List<byte[]> extModules;

		// Token: 0x02000049 RID: 73
		private struct ObfuscationAttributeInfo
		{
			// Token: 0x0400014B RID: 331
			public bool? ApplyToMembers;

			// Token: 0x0400014C RID: 332
			public bool? Exclude;

			// Token: 0x0400014D RID: 333
			public string FeatureName;

			// Token: 0x0400014E RID: 334
			public string FeatureValue;
		}

		// Token: 0x0200004A RID: 74
		private struct ProtectionSettingsInfo
		{
			// Token: 0x0400014F RID: 335
			public bool ApplyToMember;

			// Token: 0x04000150 RID: 336
			public bool Exclude;

			// Token: 0x04000151 RID: 337
			public string Settings;
		}

		// Token: 0x0200004B RID: 75
		private class ProtectionSettingsStack
		{
			// Token: 0x060001C1 RID: 449 RVA: 0x0000ED01 File Offset: 0x0000CF01
			public ProtectionSettingsStack()
			{
				this.stack = new Stack<ObfAttrMarker.ProtectionSettingsInfo[]>();
			}

			// Token: 0x060001C2 RID: 450 RVA: 0x0000ED14 File Offset: 0x0000CF14
			public ProtectionSettingsStack(ObfAttrMarker.ProtectionSettingsStack copy)
			{
				this.stack = new Stack<ObfAttrMarker.ProtectionSettingsInfo[]>(copy.stack);
			}

			// Token: 0x060001C3 RID: 451 RVA: 0x0000ED2D File Offset: 0x0000CF2D
			public void Push(IEnumerable<ObfAttrMarker.ProtectionSettingsInfo> infos)
			{
				this.stack.Push(infos.ToArray<ObfAttrMarker.ProtectionSettingsInfo>());
			}

			// Token: 0x060001C4 RID: 452 RVA: 0x0000ED40 File Offset: 0x0000CF40
			public void Pop()
			{
				this.stack.Pop();
			}

			// Token: 0x060001C5 RID: 453 RVA: 0x0000ED51 File Offset: 0x0000CF51
			public IEnumerable<ObfAttrMarker.ProtectionSettingsInfo> GetInfos()
			{
				return this.stack.Reverse<ObfAttrMarker.ProtectionSettingsInfo[]>().SelectMany((ObfAttrMarker.ProtectionSettingsInfo[] infos) => infos);
			}

			// Token: 0x04000152 RID: 338
			private readonly Stack<ObfAttrMarker.ProtectionSettingsInfo[]> stack;
		}
	}
}
