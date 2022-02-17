#define DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using KoiVM.AST;
using KoiVM.AST.ILAST;

namespace KoiVM.ILAST.Transformation
{
	public class BranchTransform : ITransformationHandler
	{
		private static readonly Dictionary<Code, Tuple<Code, Code, Code>> transformMap = new Dictionary<Code, Tuple<Code, Code, Code>>
		{
			{
				Code.Beq,
				Tuple.Create(Code.Ceq, Code.Ceq, Code.Brtrue)
			},
			{
				Code.Bne_Un,
				Tuple.Create(Code.Ceq, Code.Ceq, Code.Brfalse)
			},
			{
				Code.Bge,
				Tuple.Create(Code.Clt, Code.Clt_Un, Code.Brfalse)
			},
			{
				Code.Bge_Un,
				Tuple.Create(Code.Clt_Un, Code.Clt, Code.Brfalse)
			},
			{
				Code.Ble,
				Tuple.Create(Code.Cgt, Code.Cgt_Un, Code.Brfalse)
			},
			{
				Code.Ble_Un,
				Tuple.Create(Code.Cgt_Un, Code.Cgt, Code.Brfalse)
			},
			{
				Code.Bgt,
				Tuple.Create(Code.Cgt, Code.Cgt, Code.Brtrue)
			},
			{
				Code.Bgt_Un,
				Tuple.Create(Code.Cgt_Un, Code.Cgt_Un, Code.Brtrue)
			},
			{
				Code.Blt,
				Tuple.Create(Code.Clt, Code.Clt, Code.Brtrue)
			},
			{
				Code.Blt_Un,
				Tuple.Create(Code.Clt_Un, Code.Clt_Un, Code.Brtrue)
			}
		};

		public void Initialize(ILASTTransformer tr)
		{
		}

		public void Transform(ILASTTransformer tr)
		{
			tr.Tree.TraverseTree(Transform, tr.Method.Module);
		}

		private static void Transform(ILASTExpression expr, ModuleDef module)
		{
			Code iLCode = expr.ILCode;
			if (iLCode - 59 <= Code.Ldloc_3)
			{
				Debug.Assert(expr.Arguments.Length == 2);
				Tuple<Code, Code, Code> mapInfo = transformMap[expr.ILCode];
				Code compCode = (expr.Arguments.Any((IILASTNode arg) => arg.Type.Value == ASTType.R4 || arg.Type.Value == ASTType.R8) ? mapInfo.Item2 : mapInfo.Item1);
				expr.ILCode = mapInfo.Item3;
				expr.Arguments = new IILASTNode[1]
				{
					new ILASTExpression
					{
						ILCode = compCode,
						Arguments = expr.Arguments,
						Type = ASTType.I4
					}
				};
			}
		}
	}
}
