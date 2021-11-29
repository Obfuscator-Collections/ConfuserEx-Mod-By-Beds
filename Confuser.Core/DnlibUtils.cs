using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Core
{
	/// <summary>
	///     Provides a set of utility methods about dnlib
	/// </summary>
	// Token: 0x0200003D RID: 61
	public static class DnlibUtils
	{
		/// <summary>
		///     Finds all definitions of interest in a module.
		/// </summary>
		/// <param name="module">The module.</param>
		/// <returns>A collection of all required definitions</returns>
		// Token: 0x06000149 RID: 329 RVA: 0x0000B284 File Offset: 0x00009484
		public static IEnumerable<IDnlibDef> FindDefinitions(this ModuleDef module)
		{
			yield return module;
			foreach (TypeDef current in module.GetTypes())
			{
				yield return current;
				foreach (MethodDef current2 in current.Methods)
				{
					yield return current2;
				}
				foreach (FieldDef current3 in current.Fields)
				{
					yield return current3;
				}
				foreach (PropertyDef current4 in current.Properties)
				{
					yield return current4;
				}
				foreach (EventDef current5 in current.Events)
				{
					yield return current5;
				}
			}
			yield break;
		}

		/// <summary>
		///     Finds all definitions of interest in a type.
		/// </summary>
		/// <param name="typeDef">The type.</param>
		/// <returns>A collection of all required definitions</returns>
		// Token: 0x0600014A RID: 330 RVA: 0x0000B748 File Offset: 0x00009948
		public static IEnumerable<IDnlibDef> FindDefinitions(this TypeDef typeDef)
		{
			yield return typeDef;
			foreach (TypeDef current in typeDef.NestedTypes)
			{
				yield return current;
			}
			foreach (MethodDef current2 in typeDef.Methods)
			{
				yield return current2;
			}
			foreach (FieldDef current3 in typeDef.Fields)
			{
				yield return current3;
			}
			foreach (PropertyDef current4 in typeDef.Properties)
			{
				yield return current4;
			}
			foreach (EventDef current5 in typeDef.Events)
			{
				yield return current5;
			}
			yield break;
		}

		/// <summary>
		///     Determines whether the specified type is visible outside the containing assembly.
		/// </summary>
		/// <param name="typeDef">The type.</param>
		/// <param name="exeNonPublic">Visibility of executable modules.</param>
		/// <returns><c>true</c> if the specified type is visible outside the containing assembly; otherwise, <c>false</c>.</returns>
		// Token: 0x0600014B RID: 331 RVA: 0x0000B768 File Offset: 0x00009968
		public static bool IsVisibleOutside(this TypeDef typeDef, bool exeNonPublic = true)
		{
			if (exeNonPublic && (typeDef.Module.Kind == ModuleKind.Windows || typeDef.Module.Kind == ModuleKind.Console))
			{
				return false;
			}
			while (typeDef.DeclaringType != null)
			{
				if (!typeDef.IsNestedPublic && !typeDef.IsNestedFamily && !typeDef.IsNestedFamilyOrAssembly)
				{
					return false;
				}
				typeDef = typeDef.DeclaringType;
				if (typeDef == null)
				{
					throw new UnreachableException();
				}
			}
			return typeDef.IsPublic;
		}

		/// <summary>
		///     Determines whether the object has the specified custom attribute.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <param name="fullName">The full name of the type of custom attribute.</param>
		/// <returns><c>true</c> if the specified object has custom attribute; otherwise, <c>false</c>.</returns>
		// Token: 0x0600014C RID: 332 RVA: 0x0000B7EC File Offset: 0x000099EC
		public static bool HasAttribute(this IHasCustomAttribute obj, string fullName)
		{
			return obj.CustomAttributes.Any((CustomAttribute attr) => attr.TypeFullName == fullName);
		}

		/// <summary>
		///     Determines whether the specified type is COM import.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns><c>true</c> if specified type is COM import; otherwise, <c>false</c>.</returns>
		// Token: 0x0600014D RID: 333 RVA: 0x0000B81D File Offset: 0x00009A1D
		public static bool IsComImport(this TypeDef type)
		{
			return type.IsImport || type.HasAttribute("System.Runtime.InteropServices.ComImportAttribute") || type.HasAttribute("System.Runtime.InteropServices.TypeLibTypeAttribute");
		}

		/// <summary>
		///     Determines whether the specified type is a delegate.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns><c>true</c> if the specified type is a delegate; otherwise, <c>false</c>.</returns>
		// Token: 0x0600014E RID: 334 RVA: 0x0000B844 File Offset: 0x00009A44
		public static bool IsDelegate(this TypeDef type)
		{
			if (type.BaseType == null)
			{
				return false;
			}
			string fullName = type.BaseType.FullName;
			return fullName == "System.Delegate" || fullName == "System.MulticastDelegate";
		}

		/// <summary>
		///     Determines whether the specified type is inherited from a base type in corlib.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="baseType">The full name of base type.</param>
		/// <returns><c>true</c> if the specified type is inherited from a base type; otherwise, <c>false</c>.</returns>
		// Token: 0x0600014F RID: 335 RVA: 0x0000B884 File Offset: 0x00009A84
		public static bool InheritsFromCorlib(this TypeDef type, string baseType)
		{
			if (type.BaseType == null)
			{
				return false;
			}
			TypeDef bas = type;
			while (true)
			{
				bas = bas.BaseType.ResolveTypeDefThrow();
				if (bas.ReflectionFullName == baseType)
				{
					break;
				}
				if (bas.BaseType == null || !bas.BaseType.DefinitionAssembly.IsCorLib())
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		///     Determines whether the specified type is inherited from a base type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="baseType">The full name of base type.</param>
		/// <returns><c>true</c> if the specified type is inherited from a base type; otherwise, <c>false</c>.</returns>
		// Token: 0x06000150 RID: 336 RVA: 0x0000B8D4 File Offset: 0x00009AD4
		public static bool InheritsFrom(this TypeDef type, string baseType)
		{
			if (type.BaseType == null)
			{
				return false;
			}
			TypeDef bas = type;
			while (true)
			{
				bas = bas.BaseType.ResolveTypeDefThrow();
				if (bas.ReflectionFullName == baseType)
				{
					break;
				}
				if (bas.BaseType == null)
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		///     Determines whether the specified type implements the specified interface.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="fullName">The full name of the type of interface.</param>
		/// <returns><c>true</c> if the specified type implements the interface; otherwise, <c>false</c>.</returns>
		// Token: 0x06000151 RID: 337 RVA: 0x0000B914 File Offset: 0x00009B14
		public static bool Implements(this TypeDef type, string fullName)
		{
			while (true)
			{
				foreach (InterfaceImpl iface in type.Interfaces)
				{
					if (iface.Interface.ReflectionFullName == fullName)
					{
						return true;
					}
				}
				if (type.BaseType == null)
				{
					break;
				}
				type = type.BaseType.ResolveTypeDefThrow();
				if (type == null)
				{
					goto Block_2;
				}
			}
			return false;
			Block_2:
			throw new UnreachableException();
		}

		/// <summary>
		///     Resolves the method.
		/// </summary>
		/// <param name="method">The method to resolve.</param>
		/// <returns>A <see cref="T:dnlib.DotNet.MethodDef" /> instance.</returns>
		/// <exception cref="T:dnlib.DotNet.MemberRefResolveException">The method couldn't be resolved.</exception>
		// Token: 0x06000152 RID: 338 RVA: 0x0000B994 File Offset: 0x00009B94
		public static MethodDef ResolveThrow(this IMethod method)
		{
			MethodDef def = method as MethodDef;
			if (def != null)
			{
				return def;
			}
			MethodSpec spec = method as MethodSpec;
			if (spec != null)
			{
				return spec.Method.ResolveThrow();
			}
			return ((MemberRef)method).ResolveMethodThrow();
		}

		/// <summary>
		///     Resolves the field.
		/// </summary>
		/// <param name="field">The field to resolve.</param>
		/// <returns>A <see cref="T:dnlib.DotNet.FieldDef" /> instance.</returns>
		/// <exception cref="T:dnlib.DotNet.MemberRefResolveException">The method couldn't be resolved.</exception>
		// Token: 0x06000153 RID: 339 RVA: 0x0000B9D0 File Offset: 0x00009BD0
		public static FieldDef ResolveThrow(this IField field)
		{
			FieldDef def = field as FieldDef;
			if (def != null)
			{
				return def;
			}
			return ((MemberRef)field).ResolveFieldThrow();
		}

		/// <summary>
		///     Find the basic type reference.
		/// </summary>
		/// <param name="typeSig">The type signature to get the basic type.</param>
		/// <returns>A <see cref="T:dnlib.DotNet.ITypeDefOrRef" /> instance, or null if the typeSig cannot be resolved to basic type.</returns>
		// Token: 0x06000154 RID: 340 RVA: 0x0000B9F4 File Offset: 0x00009BF4
		public static ITypeDefOrRef ToBasicTypeDefOrRef(this TypeSig typeSig)
		{
			while (typeSig.Next != null)
			{
				typeSig = typeSig.Next;
			}
			if (typeSig is GenericInstSig)
			{
				return ((GenericInstSig)typeSig).GenericType.TypeDefOrRef;
			}
			if (typeSig is TypeDefOrRefSig)
			{
				return ((TypeDefOrRefSig)typeSig).TypeDefOrRef;
			}
			return null;
		}

		/// <summary>
		///     Find the type references within the specified type signature.
		/// </summary>
		/// <param name="typeSig">The type signature to find the type references.</param>
		/// <returns>A list of <see cref="T:dnlib.DotNet.ITypeDefOrRef" /> instance.</returns>
		// Token: 0x06000155 RID: 341 RVA: 0x0000BA44 File Offset: 0x00009C44
		public static IList<ITypeDefOrRef> FindTypeRefs(this TypeSig typeSig)
		{
			List<ITypeDefOrRef> ret = new List<ITypeDefOrRef>();
			DnlibUtils.FindTypeRefsInternal(typeSig, ret);
			return ret;
		}

		// Token: 0x06000156 RID: 342 RVA: 0x0000BA60 File Offset: 0x00009C60
		private static void FindTypeRefsInternal(TypeSig typeSig, IList<ITypeDefOrRef> ret)
		{
			while (typeSig.Next != null)
			{
				if (typeSig is ModifierSig)
				{
					ret.Add(((ModifierSig)typeSig).Modifier);
				}
				typeSig = typeSig.Next;
			}
			if (typeSig is GenericInstSig)
			{
				GenericInstSig genInst = (GenericInstSig)typeSig;
				ret.Add(genInst.GenericType.TypeDefOrRef);
				using (IEnumerator<TypeSig> enumerator = genInst.GenericArguments.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TypeSig genArg = enumerator.Current;
						DnlibUtils.FindTypeRefsInternal(genArg, ret);
					}
					return;
				}
			}
			if (typeSig is TypeDefOrRefSig)
			{
				for (ITypeDefOrRef type = ((TypeDefOrRefSig)typeSig).TypeDefOrRef; type != null; type = type.DeclaringType)
				{
					ret.Add(type);
				}
			}
		}

		/// <summary>
		///     Determines whether the specified property is public.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <returns><c>true</c> if the specified property is public; otherwise, <c>false</c>.</returns>
		// Token: 0x06000157 RID: 343 RVA: 0x0000BB28 File Offset: 0x00009D28
		public static bool IsPublic(this PropertyDef property)
		{
			if (property.GetMethod != null && property.GetMethod.IsPublic)
			{
				return true;
			}
			if (property.SetMethod != null && property.SetMethod.IsPublic)
			{
				return true;
			}
			return property.OtherMethods.Any((MethodDef method) => method.IsPublic);
		}

		/// <summary>
		///     Determines whether the specified property is static.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <returns><c>true</c> if the specified property is static; otherwise, <c>false</c>.</returns>
		// Token: 0x06000158 RID: 344 RVA: 0x0000BB94 File Offset: 0x00009D94
		public static bool IsStatic(this PropertyDef property)
		{
			if (property.GetMethod != null && property.GetMethod.IsStatic)
			{
				return true;
			}
			if (property.SetMethod != null && property.SetMethod.IsStatic)
			{
				return true;
			}
			return property.OtherMethods.Any((MethodDef method) => method.IsStatic);
		}

		/// <summary>
		///     Determines whether the specified event is public.
		/// </summary>
		/// <param name="evt">The event.</param>
		/// <returns><c>true</c> if the specified event is public; otherwise, <c>false</c>.</returns>
		// Token: 0x06000159 RID: 345 RVA: 0x0000BC00 File Offset: 0x00009E00
		public static bool IsPublic(this EventDef evt)
		{
			if (evt.AddMethod != null && evt.AddMethod.IsPublic)
			{
				return true;
			}
			if (evt.RemoveMethod != null && evt.RemoveMethod.IsPublic)
			{
				return true;
			}
			if (evt.InvokeMethod != null && evt.InvokeMethod.IsPublic)
			{
				return true;
			}
			return evt.OtherMethods.Any((MethodDef method) => method.IsPublic);
		}

		/// <summary>
		///     Determines whether the specified event is static.
		/// </summary>
		/// <param name="evt">The event.</param>
		/// <returns><c>true</c> if the specified event is static; otherwise, <c>false</c>.</returns>
		// Token: 0x0600015A RID: 346 RVA: 0x0000BC84 File Offset: 0x00009E84
		public static bool IsStatic(this EventDef evt)
		{
			if (evt.AddMethod != null && evt.AddMethod.IsStatic)
			{
				return true;
			}
			if (evt.RemoveMethod != null && evt.RemoveMethod.IsStatic)
			{
				return true;
			}
			if (evt.InvokeMethod != null && evt.InvokeMethod.IsStatic)
			{
				return true;
			}
			return evt.OtherMethods.Any((MethodDef method) => method.IsStatic);
		}

		/// <summary>
		///     Replaces the specified instruction reference with another instruction.
		/// </summary>
		/// <param name="body">The method body.</param>
		/// <param name="target">The instruction to replace.</param>
		/// <param name="newInstr">The new instruction.</param>
		// Token: 0x0600015B RID: 347 RVA: 0x0000BD00 File Offset: 0x00009F00
		public static void ReplaceReference(this CilBody body, Instruction target, Instruction newInstr)
		{
			foreach (ExceptionHandler eh in body.ExceptionHandlers)
			{
				if (eh.TryStart == target)
				{
					eh.TryStart = newInstr;
				}
				if (eh.TryEnd == target)
				{
					eh.TryEnd = newInstr;
				}
				if (eh.HandlerStart == target)
				{
					eh.HandlerStart = newInstr;
				}
				if (eh.HandlerEnd == target)
				{
					eh.HandlerEnd = newInstr;
				}
			}
			foreach (Instruction instr in body.Instructions)
			{
				if (instr.Operand == target)
				{
					instr.Operand = newInstr;
				}
				else if (instr.Operand is Instruction[])
				{
					Instruction[] targets = (Instruction[])instr.Operand;
					for (int i = 0; i < targets.Length; i++)
					{
						if (targets[i] == target)
						{
							targets[i] = newInstr;
						}
					}
				}
			}
		}
        /// <summary>
        /// Inserts Instructions
        /// </summary>
        /// <param name="instructions">list of instructions</param>
        /// <param name="keyValuePairs">Instruction:Index</param>
        public static void InsertInstructions(IList<Instruction> instructions, Dictionary<Instruction, int> keyValuePairs)
        {
            foreach (var kv in keyValuePairs)
            {
                Instruction instruction = kv.Key;
                int index = kv.Value;
                instructions.Insert(index, instruction);
            }
        }
        /// <summary>
        ///     Determines whether the specified method is array accessors.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns><c>true</c> if the specified method is array accessors; otherwise, <c>false</c>.</returns>
        // Token: 0x0600015C RID: 348 RVA: 0x0000BE08 File Offset: 0x0000A008
        public static bool IsArrayAccessors(this IMethod method)
		{
			TypeSig declType = method.DeclaringType.ToTypeSig();
			if (declType is GenericInstSig)
			{
				declType = ((GenericInstSig)declType).GenericType;
			}
			return declType.IsArray && (method.Name == "Get" || method.Name == "Set" || method.Name == "Address");
		}
	}
}
