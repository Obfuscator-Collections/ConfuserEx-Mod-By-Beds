using System;
using Confuser.Core.API;
using Confuser.Core.Services;

namespace Confuser.Core
{
	/// <summary>
	///     Core component of Confuser.
	/// </summary>
	// Token: 0x02000039 RID: 57
	public class CoreComponent : ConfuserComponent
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="T:Confuser.Core.CoreComponent" /> class.
		/// </summary>
		/// <param name="parameters">The parameters.</param>
		/// <param name="marker">The marker.</param>
		// Token: 0x06000131 RID: 305 RVA: 0x0000A6DA File Offset: 0x000088DA
		internal CoreComponent(ConfuserParameters parameters, Marker marker)
		{
			this.parameters = parameters;
			this.marker = marker;
		}

		/// <inheritdoc />
		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000132 RID: 306 RVA: 0x0000A6F0 File Offset: 0x000088F0
		public override string Name
		{
			get
			{
				return "Confuser Core";
			}
		}

		/// <inheritdoc />
		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000133 RID: 307 RVA: 0x0000A6F7 File Offset: 0x000088F7
		public override string Description
		{
			get
			{
				return "Initialization of Confuser core services.";
			}
		}

		/// <inheritdoc />
		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000134 RID: 308 RVA: 0x0000A6FE File Offset: 0x000088FE
		public override string Id
		{
			get
			{
				return "Confuser.Core";
			}
		}

		/// <inheritdoc />
		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000135 RID: 309 RVA: 0x0000A705 File Offset: 0x00008905
		public override string FullId
		{
			get
			{
				return "Confuser.Core";
			}
		}

		/// <inheritdoc />
		// Token: 0x06000136 RID: 310 RVA: 0x0000A70C File Offset: 0x0000890C
		protected internal override void Initialize(ConfuserContext context)
		{
			context.Registry.RegisterService("Confuser.Random", typeof(IRandomService), new RandomService(this.parameters.Project.Seed));
			context.Registry.RegisterService("Confuser.Marker", typeof(IMarkerService), new MarkerService(context, this.marker));
			context.Registry.RegisterService("Confuser.Trace", typeof(ITraceService), new TraceService(context));
			context.Registry.RegisterService("Confuser.Runtime", typeof(IRuntimeService), new RuntimeService());
			context.Registry.RegisterService("Confuser.Compression", typeof(ICompressionService), new CompressionService(context));
			context.Registry.RegisterService("Confuser.APIStore", typeof(IAPIStore), new APIStore(context));
		}

		/// <inheritdoc />
		// Token: 0x06000137 RID: 311 RVA: 0x0000A7ED File Offset: 0x000089ED
		protected internal override void PopulatePipeline(ProtectionPipeline pipeline)
		{
		}

		/// <summary>
		///     The service ID of RNG
		/// </summary>
		// Token: 0x04000117 RID: 279
		public const string _RandomServiceId = "Confuser.Random";

		/// <summary>
		///     The service ID of Marker
		/// </summary>
		// Token: 0x04000118 RID: 280
		public const string _MarkerServiceId = "Confuser.Marker";

		/// <summary>
		///     The service ID of Trace
		/// </summary>
		// Token: 0x04000119 RID: 281
		public const string _TraceServiceId = "Confuser.Trace";

		/// <summary>
		///     The service ID of Runtime
		/// </summary>
		// Token: 0x0400011A RID: 282
		public const string _RuntimeServiceId = "Confuser.Runtime";

		/// <summary>
		///     The service ID of Compression
		/// </summary>
		// Token: 0x0400011B RID: 283
		public const string _CompressionServiceId = "Confuser.Compression";

		/// <summary>
		///     The service ID of API Store
		/// </summary>
		// Token: 0x0400011C RID: 284
		public const string _APIStoreId = "Confuser.APIStore";

		// Token: 0x0400011D RID: 285
		private readonly Marker marker;

		// Token: 0x0400011E RID: 286
		private readonly ConfuserParameters parameters;
	}
}
