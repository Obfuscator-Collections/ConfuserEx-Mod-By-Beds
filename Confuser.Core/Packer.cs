using System;
using System.IO;
using System.Threading;
using Confuser.Core.Project;
using dnlib.DotNet;

namespace Confuser.Core
{
	/// <summary>
	///     Base class of Confuser packers.
	/// </summary>
	/// <remarks>
	///     A parameterless constructor must exists in derived classes to enable plugin discovery.
	/// </remarks>
	// Token: 0x02000050 RID: 80
	public abstract class Packer : ConfuserComponent
	{
		/// <summary>
		///     Executes the packer.
		/// </summary>
		/// <param name="context">The working context.</param>
		/// <param name="parameters">The parameters of packer.</param>
		// Token: 0x060001E7 RID: 487
		protected internal abstract void Pack(ConfuserContext context, ProtectionParameters parameters);

		/// <summary>
		///     Protects the stub using original project settings replace the current output with the protected stub.
		/// </summary>
		/// <param name="context">The working context.</param>
		/// <param name="fileName">The result file name.</param>
		/// <param name="module">The stub module.</param>
		/// <param name="snKey">The strong name key.</param>
		/// <param name="prot">The packer protection that applies to the stub.</param>
		// Token: 0x060001E8 RID: 488 RVA: 0x0000F8AC File Offset: 0x0000DAAC
		protected void ProtectStub(ConfuserContext context, string fileName, byte[] module, StrongNameKey snKey, Protection prot = null)
		{
			string tmpDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			string outDir = Path.Combine(tmpDir, Path.GetRandomFileName());
			Directory.CreateDirectory(tmpDir);
			for (int i = 0; i < context.OutputModules.Count; i++)
			{
				string path = Path.GetFullPath(Path.Combine(tmpDir, context.OutputPaths[i]));
				string dir = Path.GetDirectoryName(path);
				if (!Directory.Exists(dir))
				{
					Directory.CreateDirectory(dir);
				}
				File.WriteAllBytes(path, context.OutputModules[i]);
			}
			File.WriteAllBytes(Path.Combine(tmpDir, fileName), module);
			ConfuserProject proj = new ConfuserProject();
			proj.Seed = context.Project.Seed;
			foreach (Rule rule in context.Project.Rules)
			{
				proj.Rules.Add(rule);
			}
			proj.Add(new ProjectModule
			{
				Path = fileName
			});
			proj.BaseDirectory = tmpDir;
			proj.OutputDirectory = outDir;
			foreach (string path2 in context.Project.ProbePaths)
			{
				proj.ProbePaths.Add(path2);
			}
			proj.ProbePaths.Add(context.Project.BaseDirectory);
			PluginDiscovery discovery = null;
			if (prot != null)
			{
				Rule rule2 = new Rule("true", ProtectionPreset.None, false)
				{
					Preset = ProtectionPreset.None,
					Inherit = true,
					Pattern = "true"
				};
				rule2.Add(new SettingItem<Protection>(null, SettingItemAction.Add)
				{
					Id = prot.Id,
					Action = SettingItemAction.Add
				});
				proj.Rules.Add(rule2);
				discovery = new PackerDiscovery(prot);
			}
			try
			{
				ConfuserEngine.Run(new ConfuserParameters
				{
					Logger = new PackerLogger(context.Logger),
					PluginDiscovery = discovery,
					Marker = new PackerMarker(snKey),
					Project = proj,
					PackerInitiated = true
				}, new CancellationToken?(context.token)).Wait();
			}
			catch (AggregateException ex)
			{
				context.Logger.Error("Failed to protect packer stub.");
				throw new ConfuserException(ex);
			}
			context.OutputModules = new byte[][]
			{
				File.ReadAllBytes(Path.Combine(outDir, fileName))
			};
			context.OutputPaths = new string[]
			{
				fileName
			};
		}
	}
}
