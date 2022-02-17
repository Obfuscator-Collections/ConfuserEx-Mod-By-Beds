using System.Collections.Generic;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace KoiVM.RT.Mutation
{
	public class Renamer
	{
		private readonly Dictionary<string, string> nameMap = new Dictionary<string, string>();

		private int next;

		public Renamer(int seed)
		{
			next = seed;
		}

		private string ToString(int id)
		{
			return id.ToString("x");
		}

		private string NewName(string name)
		{
			if (!nameMap.TryGetValue(name, out var newName))
			{
				newName = (nameMap[name] = ToString(next));
				next = next * 1664525 + 1013904223;
			}
			return newName;
		}

		public void Process(ModuleDef module)
		{
			foreach (TypeDef type in module.GetTypes())
			{
				if (!type.IsPublic)
				{
					type.Namespace = "";
					type.Name = NewName(type.FullName);
				}
				foreach (GenericParam genParam in type.GenericParameters)
				{
					genParam.Name = "";
				}
				bool isDelegate = type.BaseType != null && (type.BaseType.FullName == "System.Delegate" || type.BaseType.FullName == "System.MulticastDelegate");
				foreach (MethodDef method in type.Methods)
				{
					if (method.HasBody)
					{
						foreach (Instruction instr in method.Body.Instructions)
						{
							MemberRef memberRef = instr.Operand as MemberRef;
							if (memberRef != null)
							{
								TypeDef typeDef = memberRef.DeclaringType.ResolveTypeDef();
								if (memberRef.IsMethodRef && typeDef != null && (typeDef.ResolveMethod(memberRef)?.IsRuntimeSpecialName ?? false))
								{
									typeDef = null;
								}
								if (typeDef != null && typeDef.Module == module)
								{
									memberRef.Name = NewName(memberRef.Name);
								}
							}
						}
					}
					foreach (Parameter arg in (IEnumerable<Parameter>)method.Parameters)
					{
						arg.Name = "";
					}
					if (!(method.IsRuntimeSpecialName || isDelegate) && !type.IsPublic)
					{
						method.Name = NewName(method.Name);
						method.CustomAttributes.Clear();
					}
				}
				for (int i = 0; i < type.Fields.Count; i++)
				{
					FieldDef field = type.Fields[i];
					if (field.IsLiteral)
					{
						type.Fields.RemoveAt(i--);
					}
					else if (!field.IsRuntimeSpecialName)
					{
						field.Name = NewName(field.Name);
					}
				}
				type.Properties.Clear();
				type.Events.Clear();
				type.CustomAttributes.Clear();
			}
		}
	}
}
