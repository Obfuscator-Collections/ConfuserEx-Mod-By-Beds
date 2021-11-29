using System;
using Confuser.Core.Project;

namespace Confuser.Core
{
	/// <summary>
	///     Parameters that passed to <see cref="T:Confuser.Core.ConfuserEngine" />.
	/// </summary>
	// Token: 0x02000037 RID: 55
	public class ConfuserParameters
	{
		/// <summary>
		///     Gets or sets the project that would be processed.
		/// </summary>
		/// <value>The Confuser project.</value>
		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600011C RID: 284 RVA: 0x0000A642 File Offset: 0x00008842
		// (set) Token: 0x0600011D RID: 285 RVA: 0x0000A64A File Offset: 0x0000884A
		public ConfuserProject Project
		{
			get;
			set;
		}

		/// <summary>
		///     Gets or sets the logger that used to log the protection process.
		/// </summary>
		/// <value>The logger, or <c>null</c> if logging is not needed.</value>
		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600011E RID: 286 RVA: 0x0000A653 File Offset: 0x00008853
		// (set) Token: 0x0600011F RID: 287 RVA: 0x0000A65B File Offset: 0x0000885B
		public ILogger Logger
		{
			get;
			set;
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000120 RID: 288 RVA: 0x0000A664 File Offset: 0x00008864
		// (set) Token: 0x06000121 RID: 289 RVA: 0x0000A66C File Offset: 0x0000886C
		internal bool PackerInitiated
		{
			get;
			set;
		}

		/// <summary>
		///     Gets or sets the plugin discovery service.
		/// </summary>
		/// <value>The plugin discovery service, or <c>null</c> if default discovery is used.</value>
		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000122 RID: 290 RVA: 0x0000A675 File Offset: 0x00008875
		// (set) Token: 0x06000123 RID: 291 RVA: 0x0000A67D File Offset: 0x0000887D
		public PluginDiscovery PluginDiscovery
		{
			get;
			set;
		}

		/// <summary>
		///     Gets or sets the marker.
		/// </summary>
		/// <value>The marker, or <c>null</c> if default marker is used.</value>
		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000124 RID: 292 RVA: 0x0000A686 File Offset: 0x00008886
		// (set) Token: 0x06000125 RID: 293 RVA: 0x0000A68E File Offset: 0x0000888E
		public Marker Marker
		{
			get;
			set;
		}

		/// <summary>
		///     Gets the actual non-null logger.
		/// </summary>
		/// <returns>The logger.</returns>
		// Token: 0x06000126 RID: 294 RVA: 0x0000A697 File Offset: 0x00008897
		internal ILogger GetLogger()
		{
			return this.Logger ?? NullLogger.Instance;
		}

		/// <summary>
		///     Gets the actual non-null plugin discovery service.
		/// </summary>
		/// <returns>The plugin discovery service.</returns>
		// Token: 0x06000127 RID: 295 RVA: 0x0000A6A8 File Offset: 0x000088A8
		internal PluginDiscovery GetPluginDiscovery()
		{
			return this.PluginDiscovery ?? PluginDiscovery.Instance;
		}

		/// <summary>
		///     Gets the actual non-null marker.
		/// </summary>
		/// <returns>The marker.</returns>
		// Token: 0x06000128 RID: 296 RVA: 0x0000A6B9 File Offset: 0x000088B9
		internal Marker GetMarker()
		{
			return this.Marker ?? new ObfAttrMarker();
		}
	}
}
