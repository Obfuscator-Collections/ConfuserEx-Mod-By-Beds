using System;

namespace Confuser.Core
{
	/// <summary>
	///     Various stages in <see cref="T:Confuser.Core.ProtectionPipeline" />.
	/// </summary>
	// Token: 0x0200007A RID: 122
	public enum PipelineStage
	{
		/// <summary>
		///     Confuser engine inspects the loaded modules and makes necessary changes.
		///     This stage occurs only once per pipeline run.
		/// </summary>
		// Token: 0x040001D3 RID: 467
		Inspection,
		/// <summary>
		///     Confuser engine begins to process a module.
		///     This stage occurs once per module.
		/// </summary>
		// Token: 0x040001D4 RID: 468
		BeginModule,
		/// <summary>
		///     Confuser engine processes a module.
		///     This stage occurs once per module.
		/// </summary>
		// Token: 0x040001D5 RID: 469
		ProcessModule,
		/// <summary>
		///     Confuser engine optimizes opcodes of the method bodys.
		///     This stage occurs once per module.
		/// </summary>
		// Token: 0x040001D6 RID: 470
		OptimizeMethods,
		/// <summary>
		///     Confuser engine finishes processing a module.
		///     This stage occurs once per module.
		/// </summary>
		// Token: 0x040001D7 RID: 471
		EndModule,
		/// <summary>
		///     Confuser engine writes the module to byte array.
		///     This stage occurs once per module, after all processing of modules are completed.
		/// </summary>
		// Token: 0x040001D8 RID: 472
		WriteModule,
		/// <summary>
		///     Confuser engine generates debug symbols.
		///     This stage occurs only once per pipeline run.
		/// </summary>
		// Token: 0x040001D9 RID: 473
		Debug,
		/// <summary>
		///     Confuser engine packs up the output if packer is present.
		///     This stage occurs only once per pipeline run.
		/// </summary>
		// Token: 0x040001DA RID: 474
		Pack,
		/// <summary>
		///     Confuser engine saves the output.
		///     This stage occurs only once per pipeline run.
		/// </summary>
		// Token: 0x040001DB RID: 475
		SaveModules
	}
}
