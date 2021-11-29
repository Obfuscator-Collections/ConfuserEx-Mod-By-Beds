using System;
using System.Collections.Generic;
using dnlib.DotNet;

namespace Confuser.Core
{
	/// <summary>
	///     Result of the marker.
	/// </summary>
	// Token: 0x02000044 RID: 68
	public class MarkerResult
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="T:Confuser.Core.MarkerResult" /> class.
		/// </summary>
		/// <param name="modules">The modules.</param>
		/// <param name="packer">The packer.</param>
		/// <param name="extModules">The external modules.</param>
		// Token: 0x0600019E RID: 414 RVA: 0x0000D5A2 File Offset: 0x0000B7A2
		public MarkerResult(IList<ModuleDefMD> modules, Packer packer, IList<byte[]> extModules)
		{
			this.Modules = modules;
			this.Packer = packer;
			this.ExternalModules = extModules;
		}

		/// <summary>
		///     Gets a list of modules that is marked.
		/// </summary>
		/// <value>The list of modules.</value>
		// Token: 0x17000023 RID: 35
		// (get) Token: 0x0600019F RID: 415 RVA: 0x0000D5BF File Offset: 0x0000B7BF
		// (set) Token: 0x060001A0 RID: 416 RVA: 0x0000D5C7 File Offset: 0x0000B7C7
		public IList<ModuleDefMD> Modules
		{
			get;
			private set;
		}

		/// <summary>
		///     Gets a list of external modules.
		/// </summary>
		/// <value>The list of external modules.</value>
		// Token: 0x17000024 RID: 36
		// (get) Token: 0x060001A1 RID: 417 RVA: 0x0000D5D0 File Offset: 0x0000B7D0
		// (set) Token: 0x060001A2 RID: 418 RVA: 0x0000D5D8 File Offset: 0x0000B7D8
		public IList<byte[]> ExternalModules
		{
			get;
			private set;
		}

		/// <summary>
		///     Gets the packer if exists.
		/// </summary>
		/// <value>The packer, or null if no packer exists.</value>
		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060001A3 RID: 419 RVA: 0x0000D5E1 File Offset: 0x0000B7E1
		// (set) Token: 0x060001A4 RID: 420 RVA: 0x0000D5E9 File Offset: 0x0000B7E9
		public Packer Packer
		{
			get;
			private set;
		}
	}
}
