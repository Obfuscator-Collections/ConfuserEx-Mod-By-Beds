using dnlib.DotNet;
using dnlib.DotNet.Emit;
using KoiVM.AST;
using KoiVM.AST.ILAST;

namespace KoiVM.ILAST.Transformation
{
	public class ArrayTransform : ITransformationHandler
	{
		public void Initialize(ILASTTransformer tr)
		{
		}

		public void Transform(ILASTTransformer tr)
		{
			ModuleDef module = tr.Method.Module;
			tr.Tree.TraverseTree(Transform, module);
			for (int i = 0; i < tr.Tree.Count; i++)
			{
				IILASTStatement st = tr.Tree[i];
				ILASTExpression expr = VariableInlining.GetExpression(st);
				if (expr != null)
				{
					switch (expr.ILCode)
					{
					case Code.Stelem:
						TransformSTELEM(expr, module, (ITypeDefOrRef)expr.Operand, tr.Tree, ref i);
						break;
					case Code.Stelem_I1:
						TransformSTELEM(expr, module, module.CorLibTypes.SByte.ToTypeDefOrRef(), tr.Tree, ref i);
						break;
					case Code.Stelem_I2:
						TransformSTELEM(expr, module, module.CorLibTypes.Int16.ToTypeDefOrRef(), tr.Tree, ref i);
						break;
					case Code.Stelem_I4:
						TransformSTELEM(expr, module, module.CorLibTypes.Int32.ToTypeDefOrRef(), tr.Tree, ref i);
						break;
					case Code.Stelem_I8:
						TransformSTELEM(expr, module, module.CorLibTypes.Int64.ToTypeDefOrRef(), tr.Tree, ref i);
						break;
					case Code.Stelem_R4:
						TransformSTELEM(expr, module, module.CorLibTypes.Single.ToTypeDefOrRef(), tr.Tree, ref i);
						break;
					case Code.Stelem_R8:
						TransformSTELEM(expr, module, module.CorLibTypes.Double.ToTypeDefOrRef(), tr.Tree, ref i);
						break;
					case Code.Stelem_I:
						TransformSTELEM(expr, module, module.CorLibTypes.IntPtr.ToTypeDefOrRef(), tr.Tree, ref i);
						break;
					case Code.Stelem_Ref:
						TransformSTELEM(expr, module, module.CorLibTypes.Object.ToTypeDefOrRef(), tr.Tree, ref i);
						break;
					}
				}
			}
		}

		private static void Transform(ILASTExpression expr, ModuleDef module)
		{
			switch (expr.ILCode)
			{
			case Code.Ldlen:
			{
				expr.ILCode = Code.Call;
				TypeRef array2 = module.CorLibTypes.GetTypeRef("System", "Array");
				MethodSig lenSig = MethodSig.CreateInstance(module.CorLibTypes.Int32);
				MemberRefUser methodRef = (MemberRefUser)(expr.Operand = new MemberRefUser(module, "get_Length", lenSig, array2));
				break;
			}
			case Code.Newarr:
			{
				expr.ILCode = Code.Newobj;
				ITypeDefOrRef array = new SZArraySig(((ITypeDefOrRef)expr.Operand).ToTypeSig()).ToTypeDefOrRef();
				MethodSig ctorSig = MethodSig.CreateInstance(module.CorLibTypes.Void, module.CorLibTypes.Int32);
				MemberRefUser ctorRef = (MemberRefUser)(expr.Operand = new MemberRefUser(module, ".ctor", ctorSig, array));
				break;
			}
			case Code.Ldelema:
			{
				expr.ILCode = Code.Call;
				TypeSig elemType = ((ITypeDefOrRef)expr.Operand).ToTypeSig();
				ITypeDefOrRef array3 = new SZArraySig(elemType).ToTypeDefOrRef();
				MethodSig addrSig = MethodSig.CreateInstance(new ByRefSig(elemType), module.CorLibTypes.Int32);
				MemberRefUser addrRef = (MemberRefUser)(expr.Operand = new MemberRefUser(module, "Address", addrSig, array3));
				break;
			}
			case Code.Ldelem:
				TransformLDELEM(expr, module, (ITypeDefOrRef)expr.Operand);
				break;
			case Code.Ldelem_I1:
				TransformLDELEM(expr, module, module.CorLibTypes.SByte.ToTypeDefOrRef());
				break;
			case Code.Ldelem_U1:
				TransformLDELEM(expr, module, module.CorLibTypes.Byte.ToTypeDefOrRef());
				break;
			case Code.Ldelem_I2:
				TransformLDELEM(expr, module, module.CorLibTypes.Int16.ToTypeDefOrRef());
				break;
			case Code.Ldelem_U2:
				TransformLDELEM(expr, module, module.CorLibTypes.UInt16.ToTypeDefOrRef());
				break;
			case Code.Ldelem_I4:
				TransformLDELEM(expr, module, module.CorLibTypes.Int32.ToTypeDefOrRef());
				break;
			case Code.Ldelem_U4:
				TransformLDELEM(expr, module, module.CorLibTypes.UInt32.ToTypeDefOrRef());
				break;
			case Code.Ldelem_I8:
				TransformLDELEM(expr, module, module.CorLibTypes.Int64.ToTypeDefOrRef());
				break;
			case Code.Ldelem_R4:
				TransformLDELEM(expr, module, module.CorLibTypes.Single.ToTypeDefOrRef());
				break;
			case Code.Ldelem_R8:
				TransformLDELEM(expr, module, module.CorLibTypes.Double.ToTypeDefOrRef());
				break;
			case Code.Ldelem_I:
				TransformLDELEM(expr, module, module.CorLibTypes.IntPtr.ToTypeDefOrRef());
				break;
			case Code.Ldelem_Ref:
				TransformLDELEM(expr, module, module.CorLibTypes.Object.ToTypeDefOrRef());
				break;
			case Code.Stelem_I:
			case Code.Stelem_I1:
			case Code.Stelem_I2:
			case Code.Stelem_I4:
			case Code.Stelem_I8:
			case Code.Stelem_R4:
			case Code.Stelem_R8:
			case Code.Stelem_Ref:
				break;
			}
		}

		private static void TransformLDELEM(ILASTExpression expr, ModuleDef module, ITypeDefOrRef type)
		{
			TypeRef array = module.CorLibTypes.GetTypeRef("System", "Array");
			MethodSig getValSig = MethodSig.CreateInstance(module.CorLibTypes.Object, module.CorLibTypes.Int32);
			MemberRefUser getValRef = new MemberRefUser(module, "GetValue", getValSig, array);
			ILASTExpression getValue = new ILASTExpression
			{
				ILCode = Code.Call,
				Operand = getValRef,
				Arguments = expr.Arguments
			};
			expr.ILCode = Code.Unbox_Any;
			expr.Operand = (type.IsValueType ? module.CorLibTypes.Object.ToTypeDefOrRef() : type);
			expr.Type = TypeInference.ToASTType(type.ToTypeSig());
			expr.Arguments = new IILASTNode[1] { getValue };
		}

		private static void TransformSTELEM(ILASTExpression expr, ModuleDef module, ITypeDefOrRef type, ILASTTree tree, ref int index)
		{
			TypeRef array = module.CorLibTypes.GetTypeRef("System", "Array");
			MethodSig setValSig = MethodSig.CreateInstance(module.CorLibTypes.Void, module.CorLibTypes.Object, module.CorLibTypes.Int32);
			MemberRefUser setValRef = new MemberRefUser(module, "SetValue", setValSig, array);
			ILASTVariable tmpVar1;
			if (expr.Arguments[1] is ILASTVariable)
			{
				tmpVar1 = (ILASTVariable)expr.Arguments[1];
			}
			else
			{
				tmpVar1 = new ILASTVariable
				{
					Name = $"arr_{expr.CILInstr.Offset:x4}_1",
					VariableType = ILASTVariableType.StackVar
				};
				tree.Insert(index++, new ILASTAssignment
				{
					Variable = tmpVar1,
					Value = (ILASTExpression)expr.Arguments[1]
				});
			}
			ILASTVariable tmpVar2;
			if (expr.Arguments[2] is ILASTVariable)
			{
				tmpVar2 = (ILASTVariable)expr.Arguments[2];
			}
			else
			{
				tmpVar2 = new ILASTVariable
				{
					Name = $"arr_{expr.CILInstr.Offset:x4}_2",
					VariableType = ILASTVariableType.StackVar
				};
				tree.Insert(index++, new ILASTAssignment
				{
					Variable = tmpVar2,
					Value = (ILASTExpression)expr.Arguments[2]
				});
			}
			if (type.IsPrimitive)
			{
				ILASTExpression iLASTExpression = new ILASTExpression();
				iLASTExpression.ILCode = Code.Box;
				iLASTExpression.Operand = type;
				IILASTNode[] array3 = (iLASTExpression.Arguments = new ILASTVariable[1] { tmpVar2 });
				ILASTExpression elem = iLASTExpression;
				expr.Arguments[2] = tmpVar1;
				expr.Arguments[1] = elem;
			}
			else
			{
				expr.Arguments[2] = tmpVar1;
				expr.Arguments[1] = tmpVar2;
			}
			expr.ILCode = Code.Call;
			expr.Operand = setValRef;
		}
	}
}
