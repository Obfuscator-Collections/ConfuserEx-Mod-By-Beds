using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Confuser.Core.Project;
using Confuser.Core.Project.Patterns;
using dnlib.DotNet;

namespace Confuser.Core
{
	/// <summary>
	///     Resolves and marks the modules with protection settings according to the rules.
	/// </summary>
	// Token: 0x02000043 RID: 67
	public class Marker
	{
		/// <summary>
		///     Initalizes the Marker with specified protections and packers.
		/// </summary>
		/// <param name="protections">The protections.</param>
		/// <param name="packers">The packers.</param>
		// Token: 0x06000190 RID: 400 RVA: 0x0000CE48 File Offset: 0x0000B048
		public virtual void Initalize(IList<Protection> protections, IList<Packer> packers)
		{
			this.protections = protections.ToDictionary((Protection prot) => prot.Id, (Protection prot) => prot, StringComparer.OrdinalIgnoreCase);
			this.packers = packers.ToDictionary((Packer packer) => packer.Id, (Packer packer) => packer, StringComparer.OrdinalIgnoreCase);
		}

		/// <summary>
		///     Fills the protection settings with the specified preset.
		/// </summary>
		/// <param name="preset">The preset.</param>
		/// <param name="settings">The settings.</param>
		// Token: 0x06000191 RID: 401 RVA: 0x0000CEEC File Offset: 0x0000B0EC
		private void FillPreset(ProtectionPreset preset, ProtectionSettings settings)
		{
			foreach (Protection prot in this.protections.Values)
			{
				if (prot.Preset <= preset && !settings.ContainsKey(prot))
				{
					settings.Add(prot, new Dictionary<string, string>());
				}
			}
		}

		/// <summary>
		///     Loads the Strong Name Key at the specified path with a optional password.
		/// </summary>
		/// <param name="context">The working context.</param>
		/// <param name="path">The path to the key.</param>
		/// <param name="pass">
		///     The password of the certificate at <paramref name="path" /> if
		///     it is a pfx file; otherwise, <c>null</c>.
		/// </param>
		/// <returns>The loaded Strong Name Key.</returns>
		// Token: 0x06000192 RID: 402 RVA: 0x0000CF5C File Offset: 0x0000B15C
		public static StrongNameKey LoadSNKey(ConfuserContext context, string path, string pass)
		{
			if (path == null)
			{
				return null;
			}
			StrongNameKey result;
			try
			{
				if (pass != null)
				{
					X509Certificate2 cert = new X509Certificate2();
					cert.Import(path, pass, X509KeyStorageFlags.Exportable);
					RSACryptoServiceProvider rsa = cert.PrivateKey as RSACryptoServiceProvider;
					if (rsa == null)
					{
						throw new ArgumentException("RSA key does not present in the certificate.", "path");
					}
					result = new StrongNameKey(rsa.ExportCspBlob(true));
				}
				else
				{
					result = new StrongNameKey(path);
				}
			}
			catch (Exception ex)
			{
				context.Logger.ErrorException("Cannot load the Strong Name Key located at: " + path, ex);
				throw new ConfuserException(ex);
			}
			return result;
		}

		/// <summary>
		///     Loads the assembly and marks the project.
		/// </summary>
		/// <param name="proj">The project.</param>
		/// <param name="context">The working context.</param>
		/// <returns><see cref="T:Confuser.Core.MarkerResult" /> storing the marked modules and packer information.</returns>
		// Token: 0x06000193 RID: 403 RVA: 0x0000CFF0 File Offset: 0x0000B1F0
		protected internal virtual MarkerResult MarkProject(ConfuserProject proj, ConfuserContext context)
		{
			Packer packer = null;
			Dictionary<string, string> packerParams = null;
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
				if (proj.Debug)
				{
					context.Logger.Warn("Generated Debug symbols might not be usable with packers!");
				}
				packer = this.packers[proj.Packer.Id];
				packerParams = new Dictionary<string, string>(proj.Packer, StringComparer.OrdinalIgnoreCase);
			}
			List<Tuple<ProjectModule, ModuleDefMD>> modules = new List<Tuple<ProjectModule, ModuleDefMD>>();
			List<byte[]> extModules = new List<byte[]>();
			foreach (ProjectModule module3 in proj)
			{
				if (module3.IsExternal)
				{
					extModules.Add(module3.LoadRaw(proj.BaseDirectory));
				}
				else
				{
					ModuleDefMD modDef = module3.Resolve(proj.BaseDirectory, context.Resolver.DefaultModuleContext);
					context.CheckCancellation();
					if (proj.Debug)
					{
						modDef.LoadPdb();
					}
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
				Dictionary<Rule, PatternExpression> rules = this.ParseRules(proj, module2.Item1, context);
				context.Annotations.Set<StrongNameKey>(module2.Item2, Marker.SNKey, Marker.LoadSNKey(context, (module2.Item1.SNKeyPath == null) ? null : Path.Combine(proj.BaseDirectory, module2.Item1.SNKeyPath), module2.Item1.SNKeyPassword));
				context.Annotations.Set<Dictionary<Rule, PatternExpression>>(module2.Item2, Marker.RulesKey, rules);
				foreach (IDnlibDef def in module2.Item2.FindDefinitions())
				{
					this.ApplyRules(context, def, rules, null);
					context.CheckCancellation();
				}
				if (packerParams != null)
				{
					ProtectionParameters.GetParameters(context, module2.Item2)[packer] = packerParams;
				}
			}
			return new MarkerResult((from module in modules
			select module.Item2).ToList<ModuleDefMD>(), packer, extModules);
		}

		/// <summary>
		///     Marks the member definition.
		/// </summary>
		/// <param name="member">The member definition.</param>
		/// <param name="context">The working context.</param>
		// Token: 0x06000194 RID: 404 RVA: 0x0000D2E8 File Offset: 0x0000B4E8
		protected internal virtual void MarkMember(IDnlibDef member, ConfuserContext context)
		{
			ModuleDef module = ((IMemberRef)member).Module;
			Dictionary<Rule, PatternExpression> rules = context.Annotations.Get<Dictionary<Rule, PatternExpression>>(module, Marker.RulesKey, null);
			this.ApplyRules(context, member, rules, null);
		}

		/// <summary>
		///     Parses the rules' patterns.
		/// </summary>
		/// <param name="proj">The project.</param>
		/// <param name="module">The module description.</param>
		/// <param name="context">The working context.</param>
		/// <returns>Parsed rule patterns.</returns>
		/// <exception cref="T:System.ArgumentException">
		///     One of the rules has invalid pattern.
		/// </exception>
		// Token: 0x06000195 RID: 405 RVA: 0x0000D320 File Offset: 0x0000B520
		protected Dictionary<Rule, PatternExpression> ParseRules(ConfuserProject proj, ProjectModule module, ConfuserContext context)
		{
			Dictionary<Rule, PatternExpression> ret = new Dictionary<Rule, PatternExpression>();
			PatternParser parser = new PatternParser();
			foreach (Rule rule in proj.Rules.Concat(module.Rules))
			{
				try
				{
					ret.Add(rule, parser.Parse(rule.Pattern));
				}
				catch (InvalidPatternException ex)
				{
					context.Logger.ErrorFormat("Invalid rule pattern: " + rule.Pattern + ".", new object[]
					{
						ex
					});
					throw new ConfuserException(ex);
				}
				foreach (SettingItem<Protection> setting in rule)
				{
					if (!this.protections.ContainsKey(setting.Id))
					{
						context.Logger.ErrorFormat("Cannot find protection with ID '{0}'.", new object[]
						{
							setting.Id
						});
						throw new ConfuserException(null);
					}
				}
			}
			return ret;
		}

		/// <summary>
		///     Applies the rules to the target definition.
		/// </summary>
		/// <param name="context">The working context.</param>
		/// <param name="target">The target definition.</param>
		/// <param name="rules">The rules.</param>
		/// <param name="baseSettings">The base settings.</param>
		// Token: 0x06000196 RID: 406 RVA: 0x0000D45C File Offset: 0x0000B65C
		protected void ApplyRules(ConfuserContext context, IDnlibDef target, Dictionary<Rule, PatternExpression> rules, ProtectionSettings baseSettings = null)
		{
			ProtectionSettings ret = (baseSettings == null) ? new ProtectionSettings() : new ProtectionSettings(baseSettings);
			foreach (KeyValuePair<Rule, PatternExpression> i in rules)
			{
				if ((bool)i.Value.Evaluate(target))
				{
					if (!i.Key.Inherit)
					{
						ret.Clear();
					}
					this.FillPreset(i.Key.Preset, ret);
					foreach (SettingItem<Protection> prot in i.Key)
					{
						if (prot.Action == SettingItemAction.Add)
						{
							ret[this.protections[prot.Id]] = new Dictionary<string, string>(prot, StringComparer.OrdinalIgnoreCase);
						}
						else
						{
							ret.Remove(this.protections[prot.Id]);
						}
					}
				}
			}
			ProtectionParameters.SetParameters(context, target, ret);
		}

		/// <summary>
		///     Annotation key of Strong Name Key.
		/// </summary>
		// Token: 0x04000133 RID: 307
		public static readonly object SNKey = new object();

		/// <summary>
		///     Annotation key of rules.
		/// </summary>
		// Token: 0x04000134 RID: 308
		public static readonly object RulesKey = new object();

		/// <summary>
		///     The packers available to use.
		/// </summary>
		// Token: 0x04000135 RID: 309
		protected Dictionary<string, Packer> packers;

		/// <summary>
		///     The protections available to use.
		/// </summary>
		// Token: 0x04000136 RID: 310
		protected Dictionary<string, Protection> protections;
	}
}
