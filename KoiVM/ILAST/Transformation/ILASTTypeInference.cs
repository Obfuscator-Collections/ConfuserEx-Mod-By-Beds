#define DEBUG
using System;
using System.Diagnostics;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using KoiVM.AST;
using KoiVM.AST.ILAST;

namespace KoiVM.ILAST.Transformation
{
	public class ILASTTypeInference : ITransformationHandler
	{
		public void Initialize(ILASTTransformer tr)
		{
		}

		public void Transform(ILASTTransformer tr)
		{
			foreach (IILASTStatement st in tr.Tree)
			{
				if (st is ILASTExpression)
				{
					ProcessExpression((ILASTExpression)st);
				}
				else if (st is ILASTAssignment)
				{
					ILASTAssignment assignment = (ILASTAssignment)st;
					assignment.Variable.Type = ProcessExpression(assignment.Value).Value;
				}
				else if (st is ILASTPhi)
				{
					ProcessPhiNode((ILASTPhi)st);
				}
			}
		}

		private void ProcessPhiNode(ILASTPhi phi)
		{
			phi.Variable.Type = phi.SourceVariables[0].Type;
		}

		private ASTType? ProcessExpression(ILASTExpression expr)
		{
			IILASTNode[] arguments = expr.Arguments;
			foreach (IILASTNode arg in arguments)
			{
				if (arg is ILASTExpression)
				{
					ILASTExpression argExpr = (ILASTExpression)arg;
					argExpr.Type = ProcessExpression(argExpr).Value;
				}
			}
			ASTType? exprType = InferType(expr);
			if (exprType.HasValue)
			{
				expr.Type = exprType.Value;
			}
			return exprType;
		}

		private static ASTType? InferType(ILASTExpression expr)
		{
			if (expr.Type.HasValue)
			{
				return expr.Type;
			}
			OpCode opCode = expr.ILCode.ToOpCode();
			switch (opCode.StackBehaviourPush)
			{
			case StackBehaviour.Push1:
				return InferPush1(expr);
			case StackBehaviour.Pushi:
				return InferPushI(expr);
			case StackBehaviour.Pushi8:
				return InferPushI8(expr);
			case StackBehaviour.Pushr4:
				return InferPushR4(expr);
			case StackBehaviour.Pushr8:
				return InferPushR8(expr);
			case StackBehaviour.Pushref:
				return InferPushRef(expr);
			case StackBehaviour.Varpush:
				return InferVarPush(expr);
			case StackBehaviour.Push1_push1:
				Debug.Assert(expr.Arguments.Length == 1);
				return expr.Arguments[0].Type;
			default:
				return null;
			}
		}

		private static ASTType? InferPush1(ILASTExpression expr)
		{
			switch (expr.ILCode)
			{
			case Code.Add:
			case Code.Sub:
			case Code.Mul:
			case Code.Div:
			case Code.Div_Un:
			case Code.Rem:
			case Code.Rem_Un:
			case Code.Add_Ovf:
			case Code.Add_Ovf_Un:
			case Code.Mul_Ovf:
			case Code.Mul_Ovf_Un:
			case Code.Sub_Ovf:
			case Code.Sub_Ovf_Un:
				Debug.Assert(expr.Arguments.Length == 2);
				Debug.Assert(expr.Arguments[0].Type.HasValue && expr.Arguments[1].Type.HasValue);
				return TypeInference.InferBinaryOp(expr.Arguments[0].Type.Value, expr.Arguments[1].Type.Value);
			case Code.And:
			case Code.Or:
			case Code.Xor:
				Debug.Assert(expr.Arguments.Length == 2);
				Debug.Assert(expr.Arguments[0].Type.HasValue && expr.Arguments[1].Type.HasValue);
				return TypeInference.InferIntegerOp(expr.Arguments[0].Type.Value, expr.Arguments[1].Type.Value);
			case Code.Not:
				Debug.Assert(expr.Arguments.Length == 1 && expr.Arguments[0].Type.HasValue);
				if (expr.Arguments[0].Type != ASTType.I4 && expr.Arguments[0].Type != ASTType.I8 && expr.Arguments[0].Type != ASTType.Ptr)
				{
					throw new ArgumentException("Invalid Not Operand Types.");
				}
				return expr.Arguments[0].Type;
			case Code.Neg:
				Debug.Assert(expr.Arguments.Length == 1 && expr.Arguments[0].Type.HasValue);
				if (expr.Arguments[0].Type != ASTType.I4 && expr.Arguments[0].Type != ASTType.I8 && expr.Arguments[0].Type != ASTType.R4 && expr.Arguments[0].Type != ASTType.R8 && expr.Arguments[0].Type != ASTType.Ptr)
				{
					throw new ArgumentException("Invalid Not Operand Types.");
				}
				return expr.Arguments[0].Type;
			case Code.Shl:
			case Code.Shr:
			case Code.Shr_Un:
				Debug.Assert(expr.Arguments.Length == 2);
				Debug.Assert(expr.Arguments[0].Type.HasValue && expr.Arguments[1].Type.HasValue);
				return TypeInference.InferShiftOp(expr.Arguments[0].Type.Value, expr.Arguments[1].Type.Value);
			case Code.Mkrefany:
				return ASTType.O;
			case Code.Ldarg:
				return TypeInference.ToASTType(((Parameter)expr.Operand).Type);
			case Code.Ldloc:
				return TypeInference.ToASTType(((Local)expr.Operand).Type);
			case Code.Ldobj:
			case Code.Ldelem:
			case Code.Unbox_Any:
				return TypeInference.ToASTType(((ITypeDefOrRef)expr.Operand).ToTypeSig());
			case Code.Ldfld:
			case Code.Ldsfld:
				return TypeInference.ToASTType(((IField)expr.Operand).FieldSig.Type);
			default:
				throw new NotSupportedException(expr.ILCode.ToString());
			}
		}

		private static ASTType? InferPushI(ILASTExpression expr)
		{
			switch (expr.ILCode)
			{
			case Code.Ldind_I:
			case Code.Conv_Ovf_I_Un:
			case Code.Conv_Ovf_U_Un:
			case Code.Ldelem_I:
			case Code.Conv_I:
			case Code.Conv_Ovf_I:
			case Code.Conv_Ovf_U:
			case Code.Conv_U:
			case Code.Ldftn:
			case Code.Ldvirtftn:
			case Code.Localloc:
				return ASTType.Ptr;
			case Code.Ldflda:
			case Code.Ldsflda:
			case Code.Ldelema:
			case Code.Ldarga:
			case Code.Ldloca:
				return ASTType.ByRef;
			case Code.Isinst:
			case Code.Unbox:
			case Code.Refanyval:
			case Code.Ldtoken:
			case Code.Arglist:
			case Code.Refanytype:
				return ASTType.O;
			default:
				return ASTType.I4;
			}
		}

		private static ASTType? InferPushI8(ILASTExpression expr)
		{
			return ASTType.I8;
		}

		private static ASTType? InferPushR4(ILASTExpression expr)
		{
			return ASTType.R4;
		}

		private static ASTType? InferPushR8(ILASTExpression expr)
		{
			return ASTType.R8;
		}

		private static ASTType? InferPushRef(ILASTExpression expr)
		{
			return ASTType.O;
		}

		private static ASTType? InferVarPush(ILASTExpression expr)
		{
			IMethod method = (IMethod)expr.Operand;
			if (method.MethodSig.RetType.ElementType == ElementType.Void)
			{
				return null;
			}
			GenericArguments genArgs = new GenericArguments();
			if (method is MethodSpec)
			{
				genArgs.PushMethodArgs(((MethodSpec)method).GenericInstMethodSig.GenericArguments);
			}
			if (method.DeclaringType.TryGetGenericInstSig() != null)
			{
				genArgs.PushTypeArgs(method.DeclaringType.TryGetGenericInstSig().GenericArguments);
			}
			return TypeInference.ToASTType(genArgs.ResolveType(method.MethodSig.RetType));
		}
	}
}
