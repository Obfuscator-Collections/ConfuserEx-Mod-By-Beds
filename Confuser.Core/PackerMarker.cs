using System;
using Confuser.Core.Project;
using dnlib.DotNet;

namespace Confuser.Core
{
	// Token: 0x02000052 RID: 82
	internal class PackerMarker : Marker
	{
		// Token: 0x060001F8 RID: 504 RVA: 0x0000FC45 File Offset: 0x0000DE45
		public PackerMarker(StrongNameKey snKey)
		{
			this.snKey = snKey;
		}

		// Token: 0x060001F9 RID: 505 RVA: 0x0000FC54 File Offset: 0x0000DE54
		protected internal override MarkerResult MarkProject(ConfuserProject proj, ConfuserContext context)
		{
			MarkerResult result = base.MarkProject(proj, context);
			foreach (ModuleDefMD module in result.Modules)
			{
				context.Annotations.Set<StrongNameKey>(module, Marker.SNKey, this.snKey);
			}
			return result;
		}

		// Token: 0x04000162 RID: 354
		private readonly StrongNameKey snKey;
	}
}
