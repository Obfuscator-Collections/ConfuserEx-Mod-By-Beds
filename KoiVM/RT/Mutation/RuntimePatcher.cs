using System.Reflection;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace KoiVM.RT.Mutation
{
	[Obfuscation(Exclude = false, Feature = "+koi;-ref proxy")]
	internal static class RuntimePatcher
	{
		public static void Patch(ModuleDef runtime, bool debug, bool stackwalk)
		{
			PatchDispatcher(runtime, debug, stackwalk);
		}

		private static void PatchDispatcher(ModuleDef runtime, bool debug, bool stackwalk)
		{
			TypeDef dispatcher = runtime.Find(RTMap.VMDispatcher, isReflectionName: true);
			MethodDef dispatcherRun = dispatcher.FindMethod(RTMap.VMRun);
			foreach (ExceptionHandler eh in dispatcherRun.Body.ExceptionHandlers)
			{
				if (eh.HandlerType == ExceptionHandlerType.Catch)
				{
					eh.CatchType = runtime.CorLibTypes.Object.ToTypeDefOrRef();
				}
			}
			PatchDoThrow(dispatcher.FindMethod(RTMap.VMDispatcherDothrow).Body, debug, stackwalk);
			dispatcher.Methods.Remove(dispatcher.FindMethod(RTMap.VMDispatcherThrow));
		}

		private static void PatchDoThrow(CilBody body, bool debug, bool stackwalk)
		{
			for (int i = 0; i < body.Instructions.Count; i++)
			{
				IMethod method = body.Instructions[i].Operand as IMethod;
				if (method != null && method.Name == RTMap.VMDispatcherThrow)
				{
					body.Instructions.RemoveAt(i);
				}
				else if (method != null && method.Name == RTMap.VMDispatcherGetIP)
				{
					if (!debug)
					{
						body.Instructions.RemoveAt(i);
						body.Instructions[i - 1].OpCode = OpCodes.Ldnull;
						MethodDef def3 = method.ResolveMethodDefThrow();
						def3.DeclaringType.Methods.Remove(def3);
					}
					else if (stackwalk)
					{
						MethodDef def2 = method.ResolveMethodDefThrow();
						body.Instructions[i].Operand = def2.DeclaringType.FindMethod(RTMap.VMDispatcherStackwalk);
						def2.DeclaringType.Methods.Remove(def2);
					}
					else
					{
						MethodDef def = method.ResolveMethodDefThrow();
						def = def.DeclaringType.FindMethod(RTMap.VMDispatcherStackwalk);
						def.DeclaringType.Methods.Remove(def);
					}
				}
			}
		}
	}
}
