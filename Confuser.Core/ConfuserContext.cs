using System;
using System.Collections.Generic;
using System.Threading;
using Confuser.Core.Project;
using dnlib.DotNet;
using dnlib.DotNet.Writer;

namespace Confuser.Core
{
	/// <summary>
	///     Context providing information on the current protection process.
	/// </summary>
	// Token: 0x02000078 RID: 120
	public class ConfuserContext
	{
		/// <summary>
		///     Gets the logger used for logging events.
		/// </summary>
		/// <value>The logger.</value>
		// Token: 0x17000067 RID: 103
		// (get) Token: 0x060002C7 RID: 711 RVA: 0x0001238F File Offset: 0x0001058F
		// (set) Token: 0x060002C8 RID: 712 RVA: 0x00012397 File Offset: 0x00010597
		public ILogger Logger
		{
			get;
			internal set;
		}

		/// <summary>
		///     Gets the project being processed.
		/// </summary>
		/// <value>The project.</value>
		// Token: 0x17000068 RID: 104
		// (get) Token: 0x060002C9 RID: 713 RVA: 0x000123A0 File Offset: 0x000105A0
		// (set) Token: 0x060002CA RID: 714 RVA: 0x000123A8 File Offset: 0x000105A8
		public ConfuserProject Project
		{
			get;
			internal set;
		}

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x060002CB RID: 715 RVA: 0x000123B1 File Offset: 0x000105B1
		// (set) Token: 0x060002CC RID: 716 RVA: 0x000123B9 File Offset: 0x000105B9
		internal bool PackerInitiated
		{
			get;
			set;
		}

		/// <summary>
		///     Gets the annotation storage.
		/// </summary>
		/// <value>The annotation storage.</value>
		// Token: 0x1700006A RID: 106
		// (get) Token: 0x060002CD RID: 717 RVA: 0x000123C2 File Offset: 0x000105C2
		public Annotations Annotations
		{
			get
			{
				return this.annotations;
			}
		}

		/// <summary>
		///     Gets the service registry.
		/// </summary>
		/// <value>The service registry.</value>
		// Token: 0x1700006B RID: 107
		// (get) Token: 0x060002CE RID: 718 RVA: 0x000123CA File Offset: 0x000105CA
		public ServiceRegistry Registry
		{
			get
			{
				return this.registry;
			}
		}

		/// <summary>
		///     Gets the assembly resolver.
		/// </summary>
		/// <value>The assembly resolver.</value>
		// Token: 0x1700006C RID: 108
		// (get) Token: 0x060002CF RID: 719 RVA: 0x000123D2 File Offset: 0x000105D2
		// (set) Token: 0x060002D0 RID: 720 RVA: 0x000123DA File Offset: 0x000105DA
		public AssemblyResolver Resolver
		{
			get;
			internal set;
		}

		/// <summary>
		///     Gets the modules being protected.
		/// </summary>
		/// <value>The modules being protected.</value>
		// Token: 0x1700006D RID: 109
		// (get) Token: 0x060002D1 RID: 721 RVA: 0x000123E3 File Offset: 0x000105E3
		// (set) Token: 0x060002D2 RID: 722 RVA: 0x000123EB File Offset: 0x000105EB
		public IList<ModuleDefMD> Modules
		{
			get;
			internal set;
		}

		/// <summary>
		///     Gets the external modules.
		/// </summary>
		/// <value>The external modules.</value>
		// Token: 0x1700006E RID: 110
		// (get) Token: 0x060002D3 RID: 723 RVA: 0x000123F4 File Offset: 0x000105F4
		// (set) Token: 0x060002D4 RID: 724 RVA: 0x000123FC File Offset: 0x000105FC
		public IList<byte[]> ExternalModules
		{
			get;
			internal set;
		}

		/// <summary>
		///     Gets the base directory.
		/// </summary>
		/// <value>The base directory.</value>
		// Token: 0x1700006F RID: 111
		// (get) Token: 0x060002D5 RID: 725 RVA: 0x00012405 File Offset: 0x00010605
		// (set) Token: 0x060002D6 RID: 726 RVA: 0x0001240D File Offset: 0x0001060D
		public string BaseDirectory
		{
			get;
			internal set;
		}

		/// <summary>
		///     Gets the output directory.
		/// </summary>
		/// <value>The output directory.</value>
		// Token: 0x17000070 RID: 112
		// (get) Token: 0x060002D7 RID: 727 RVA: 0x00012416 File Offset: 0x00010616
		// (set) Token: 0x060002D8 RID: 728 RVA: 0x0001241E File Offset: 0x0001061E
		public string OutputDirectory
		{
			get;
			internal set;
		}

		/// <summary>
		///     Gets the packer.
		/// </summary>
		/// <value>The packer.</value>
		// Token: 0x17000071 RID: 113
		// (get) Token: 0x060002D9 RID: 729 RVA: 0x00012427 File Offset: 0x00010627
		// (set) Token: 0x060002DA RID: 730 RVA: 0x0001242F File Offset: 0x0001062F
		public Packer Packer
		{
			get;
			internal set;
		}

		/// <summary>
		///     Gets the current processing pipeline.
		/// </summary>
		/// <value>The processing pipeline.</value>
		// Token: 0x17000072 RID: 114
		// (get) Token: 0x060002DB RID: 731 RVA: 0x00012438 File Offset: 0x00010638
		// (set) Token: 0x060002DC RID: 732 RVA: 0x00012440 File Offset: 0x00010640
		public ProtectionPipeline Pipeline
		{
			get;
			internal set;
		}

		/// <summary>
		///     Gets the <c>byte[]</c> of modules after protected, or null if module is not protected yet.
		/// </summary>
		/// <value>The list of <c>byte[]</c> of protected modules.</value>
		// Token: 0x17000073 RID: 115
		// (get) Token: 0x060002DD RID: 733 RVA: 0x00012449 File Offset: 0x00010649
		// (set) Token: 0x060002DE RID: 734 RVA: 0x00012451 File Offset: 0x00010651
		public IList<byte[]> OutputModules
		{
			get;
			internal set;
		}

		/// <summary>
		///     Gets the <c>byte[]</c> of module debug symbols after protected, or null if module is not protected yet.
		/// </summary>
		/// <value>The list of <c>byte[]</c> of module debug symbols.</value>
		// Token: 0x17000074 RID: 116
		// (get) Token: 0x060002DF RID: 735 RVA: 0x0001245A File Offset: 0x0001065A
		// (set) Token: 0x060002E0 RID: 736 RVA: 0x00012462 File Offset: 0x00010662
		public IList<byte[]> OutputSymbols
		{
			get;
			internal set;
		}

		/// <summary>
		///     Gets the relative output paths of module, or null if module is not protected yet.
		/// </summary>
		/// <value>The relative output paths of protected modules.</value>
		// Token: 0x17000075 RID: 117
		// (get) Token: 0x060002E1 RID: 737 RVA: 0x0001246B File Offset: 0x0001066B
		// (set) Token: 0x060002E2 RID: 738 RVA: 0x00012473 File Offset: 0x00010673
		public IList<string> OutputPaths
		{
			get;
			internal set;
		}

		/// <summary>
		///     Gets the current module index.
		/// </summary>
		/// <value>The current module index.</value>
		// Token: 0x17000076 RID: 118
		// (get) Token: 0x060002E3 RID: 739 RVA: 0x0001247C File Offset: 0x0001067C
		// (set) Token: 0x060002E4 RID: 740 RVA: 0x00012484 File Offset: 0x00010684
		public int CurrentModuleIndex
		{
			get;
			internal set;
		}

		/// <summary>
		///     Gets the current module.
		/// </summary>
		/// <value>The current module.</value>
		// Token: 0x17000077 RID: 119
		// (get) Token: 0x060002E5 RID: 741 RVA: 0x0001248D File Offset: 0x0001068D
		public ModuleDefMD CurrentModule
		{
			get
			{
				if (this.CurrentModuleIndex != -1)
				{
					return this.Modules[this.CurrentModuleIndex];
				}
				return null;
			}
		}

		/// <summary>
		///     Gets the writer options of the current module.
		/// </summary>
		/// <value>The writer options.</value>
		// Token: 0x17000078 RID: 120
		// (get) Token: 0x060002E6 RID: 742 RVA: 0x000124AB File Offset: 0x000106AB
		// (set) Token: 0x060002E7 RID: 743 RVA: 0x000124B3 File Offset: 0x000106B3
		public ModuleWriterOptionsBase CurrentModuleWriterOptions
		{
			get;
			internal set;
		}

		/// <summary>
		///     Gets the writer event listener of the current module.
		/// </summary>
		/// <value>The writer event listener.</value>
		// Token: 0x17000079 RID: 121
		// (get) Token: 0x060002E8 RID: 744 RVA: 0x000124BC File Offset: 0x000106BC
		// (set) Token: 0x060002E9 RID: 745 RVA: 0x000124C4 File Offset: 0x000106C4
		public ModuleWriterListener CurrentModuleWriterListener
		{
			get;
			internal set;
		}

		/// <summary>
		///     Gets output <c>byte[]</c> of the current module
		/// </summary>
		/// <value>The output <c>byte[]</c>.</value>
		// Token: 0x1700007A RID: 122
		// (get) Token: 0x060002EA RID: 746 RVA: 0x000124CD File Offset: 0x000106CD
		// (set) Token: 0x060002EB RID: 747 RVA: 0x000124D5 File Offset: 0x000106D5
		public byte[] CurrentModuleOutput
		{
			get;
			internal set;
		}

		/// <summary>
		///     Gets output <c>byte[]</c> debug symbol of the current module
		/// </summary>
		/// <value>The output <c>byte[]</c> debug symbol.</value>
		// Token: 0x1700007B RID: 123
		// (get) Token: 0x060002EC RID: 748 RVA: 0x000124DE File Offset: 0x000106DE
		// (set) Token: 0x060002ED RID: 749 RVA: 0x000124E6 File Offset: 0x000106E6
		public byte[] CurrentModuleSymbol
		{
			get;
			internal set;
		}

		/// <summary>
		/// 	Gets the token used to indicate cancellation
		/// </summary>
		// Token: 0x1700007C RID: 124
		// (get) Token: 0x060002EE RID: 750 RVA: 0x000124EF File Offset: 0x000106EF
		public CancellationToken CancellationToken
		{
			get
			{
				return this.token;
			}
		}

		/// <summary>
		///     Throws a System.OperationCanceledException if protection process has been canceled.
		/// </summary>
		/// <exception cref="T:System.OperationCanceledException">
		///     The protection process is canceled.
		/// </exception>
		// Token: 0x060002EF RID: 751 RVA: 0x000124F7 File Offset: 0x000106F7
		public void CheckCancellation()
		{
			this.token.ThrowIfCancellationRequested();
		}

		/// <summary>
		///     Requests the current module to be written as mix-mode module, and return the native writer options.
		/// </summary>
		/// <returns>The native writer options.</returns>
		// Token: 0x060002F0 RID: 752 RVA: 0x00012504 File Offset: 0x00010704
		public NativeModuleWriterOptions RequestNative()
		{
			if (this.CurrentModule == null)
			{
				return null;
			}
			if (this.CurrentModuleWriterOptions == null)
			{
				this.CurrentModuleWriterOptions = new NativeModuleWriterOptions(this.CurrentModule);
			}
			if (this.CurrentModuleWriterOptions is NativeModuleWriterOptions)
			{
				return (NativeModuleWriterOptions)this.CurrentModuleWriterOptions;
			}
			NativeModuleWriterOptions newOptions = new NativeModuleWriterOptions(this.CurrentModule, this.CurrentModuleWriterOptions.Listener);
			newOptions.AddCheckSum = this.CurrentModuleWriterOptions.AddCheckSum;
			newOptions.Cor20HeaderOptions = this.CurrentModuleWriterOptions.Cor20HeaderOptions;
			newOptions.Logger = this.CurrentModuleWriterOptions.Logger;
			newOptions.MetaDataLogger = this.CurrentModuleWriterOptions.MetaDataLogger;
			newOptions.MetaDataOptions = this.CurrentModuleWriterOptions.MetaDataOptions;
			newOptions.ModuleKind = this.CurrentModuleWriterOptions.ModuleKind;
			newOptions.PEHeadersOptions = this.CurrentModuleWriterOptions.PEHeadersOptions;
			newOptions.ShareMethodBodies = this.CurrentModuleWriterOptions.ShareMethodBodies;
			newOptions.StrongNameKey = this.CurrentModuleWriterOptions.StrongNameKey;
			newOptions.StrongNamePublicKey = this.CurrentModuleWriterOptions.StrongNamePublicKey;
			newOptions.Win32Resources = this.CurrentModuleWriterOptions.Win32Resources;
			this.CurrentModuleWriterOptions = newOptions;
			return newOptions;
		}

		// Token: 0x040001B9 RID: 441
		private readonly Annotations annotations = new Annotations();

		// Token: 0x040001BA RID: 442
		private readonly ServiceRegistry registry = new ServiceRegistry();

		// Token: 0x040001BB RID: 443
		internal CancellationToken token;
	}
}
