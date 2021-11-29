using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;

namespace Confuser.Core
{
	/// <summary>
	///     Protection processing pipeline.
	/// </summary>
	// Token: 0x0200007B RID: 123
	public class ProtectionPipeline
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="T:Confuser.Core.ProtectionPipeline" /> class.
		/// </summary>
		// Token: 0x060002F9 RID: 761 RVA: 0x00012778 File Offset: 0x00010978
		public ProtectionPipeline()
		{
			PipelineStage[] stages = (PipelineStage[])Enum.GetValues(typeof(PipelineStage));
			this.preStage = stages.ToDictionary((PipelineStage stage) => stage, (PipelineStage stage) => new List<ProtectionPhase>());
			this.postStage = stages.ToDictionary((PipelineStage stage) => stage, (PipelineStage stage) => new List<ProtectionPhase>());
		}

		/// <summary>
		///     Inserts the phase into pre-processing pipeline of the specified stage.
		/// </summary>
		/// <param name="stage">The pipeline stage.</param>
		/// <param name="phase">The protection phase.</param>
		// Token: 0x060002FA RID: 762 RVA: 0x0001282C File Offset: 0x00010A2C
		public void InsertPreStage(PipelineStage stage, ProtectionPhase phase)
		{
			this.preStage[stage].Add(phase);
		}

		/// <summary>
		///     Inserts the phase into post-processing pipeline of the specified stage.
		/// </summary>
		/// <param name="stage">The pipeline stage.</param>
		/// <param name="phase">The protection phase.</param>
		// Token: 0x060002FB RID: 763 RVA: 0x00012840 File Offset: 0x00010A40
		public void InsertPostStage(PipelineStage stage, ProtectionPhase phase)
		{
			this.postStage[stage].Add(phase);
		}

		/// <summary>
		///     Finds the phase with the specified type in the pipeline.
		/// </summary>
		/// <typeparam name="T">The type of the phase.</typeparam>
		/// <returns>The phase with specified type in the pipeline.</returns>
		// Token: 0x060002FC RID: 764 RVA: 0x00012854 File Offset: 0x00010A54
		public T FindPhase<T>() where T : ProtectionPhase
		{
			foreach (List<ProtectionPhase> phases in this.preStage.Values)
			{
				foreach (ProtectionPhase phase in phases)
				{
					if (phase is T)
					{
						T result = (T)((object)phase);
						return result;
					}
				}
			}
			foreach (List<ProtectionPhase> phases2 in this.postStage.Values)
			{
				foreach (ProtectionPhase phase2 in phases2)
				{
					if (phase2 is T)
					{
						T result = (T)((object)phase2);
						return result;
					}
				}
			}
			return default(T);
		}

		/// <summary>
		///     Execute the specified pipeline stage with pre-processing and post-processing.
		/// </summary>
		/// <param name="stage">The pipeline stage.</param>
		/// <param name="func">The stage function.</param>
		/// <param name="targets">The target list of the stage.</param>
		/// <param name="context">The working context.</param>
		// Token: 0x060002FD RID: 765 RVA: 0x0001298C File Offset: 0x00010B8C
		internal void ExecuteStage(PipelineStage stage, Action<ConfuserContext> func, Func<IList<IDnlibDef>> targets, ConfuserContext context)
		{
			foreach (ProtectionPhase pre in this.preStage[stage])
			{
				context.CheckCancellation();
                var targetList = Filter(context, targets(), pre);
                if (targetList.Any())
                    context.Logger.DebugFormat("Executing '{0}' phase...", pre.Name);
                pre.Execute(context, new ProtectionParameters(pre.Parent, targetList));
            }
			context.CheckCancellation();
			func(context);
			context.CheckCancellation();
			foreach (ProtectionPhase post in this.postStage[stage])
			{
                var targetList = Filter(context, targets(), post);
                if (targetList.Any())
                    context.Logger.DebugFormat("Executing '{0}' phase...", post.Name);
                post.Execute(context, new ProtectionParameters(post.Parent, targetList));
                context.CheckCancellation();
			}
		}

		/// <summary>
		///     Returns only the targets with the specified type and used by specified component.
		/// </summary>
		/// <param name="context">The working context.</param>
		/// <param name="targets">List of targets.</param>
		/// <param name="phase">The component phase.</param>
		/// <returns>Filtered targets.</returns>
		// Token: 0x060002FE RID: 766 RVA: 0x00012B84 File Offset: 0x00010D84
		private static IList<IDnlibDef> Filter(ConfuserContext context, IList<IDnlibDef> targets, ProtectionPhase phase)
		{
			ProtectionTargets targetType = phase.Targets;
			IEnumerable<IDnlibDef> filter = targets;
			if ((targetType & ProtectionTargets.Modules) == (ProtectionTargets)0)
			{
				filter = from def in filter
				where !(def is ModuleDef)
				select def;
			}
			if ((targetType & ProtectionTargets.Types) == (ProtectionTargets)0)
			{
				filter = from def in filter
				where !(def is TypeDef)
				select def;
			}
			if ((targetType & ProtectionTargets.Methods) == (ProtectionTargets)0)
			{
				filter = from def in filter
				where !(def is MethodDef)
				select def;
			}
			if ((targetType & ProtectionTargets.Fields) == (ProtectionTargets)0)
			{
				filter = from def in filter
				where !(def is FieldDef)
				select def;
			}
			if ((targetType & ProtectionTargets.Properties) == (ProtectionTargets)0)
			{
				filter = from def in filter
				where !(def is PropertyDef)
				select def;
			}
			if ((targetType & ProtectionTargets.Events) == (ProtectionTargets)0)
			{
				filter = from def in filter
				where !(def is EventDef)
				select def;
			}
			if (phase.ProcessAll)
			{
				return filter.ToList<IDnlibDef>();
			}
			return filter.Where(delegate(IDnlibDef def)
			{
				ProtectionSettings parameters = ProtectionParameters.GetParameters(context, def);
				if (parameters == null)
				{
					context.Logger.ErrorFormat("'{0}' not marked for obfuscation, possibly a bug.", new object[]
					{
						def
					});
					throw new ConfuserException(null);
				}
				return parameters.ContainsKey(phase.Parent);
			}).ToList<IDnlibDef>();
		}

		// Token: 0x040001DC RID: 476
		private readonly Dictionary<PipelineStage, List<ProtectionPhase>> postStage;

		// Token: 0x040001DD RID: 477
		private readonly Dictionary<PipelineStage, List<ProtectionPhase>> preStage;
	}
}
