using System.Collections.Generic;
using System.Linq;
using System.Text;
using dnlib.DotNet.Emit;
using KoiVM.CFG;

namespace KoiVM.AST.ILAST
{
	public class ILASTExpression : ASTExpression, IILASTNode, IILASTStatement
	{
		public Code ILCode { get; set; }

		public Instruction CILInstr { get; set; }

		public object Operand { get; set; }

		public IILASTNode[] Arguments { get; set; }

		public Instruction[] Prefixes { get; set; }

		public override string ToString()
		{
			StringBuilder ret = new StringBuilder();
			ret.AppendFormat("{0}{1}(", ILCode.ToOpCode().Name, (!base.Type.HasValue) ? "" : (":" + base.Type.Value));
			if (Operand != null)
			{
				if (Operand is string)
				{
					ASTConstant.EscapeString(ret, (string)Operand, addQuotes: true);
				}
				else if (Operand is IBasicBlock)
				{
					ret.AppendFormat("Block_{0:x2}", ((IBasicBlock)Operand).Id);
				}
				else if (Operand is IBasicBlock[])
				{
					IEnumerable<string> targets = ((IBasicBlock[])Operand).Select((IBasicBlock block) => $"Block_{block.Id:x2}");
					ret.AppendFormat("[{0}]", string.Join(", ", targets));
				}
				else
				{
					ret.Append(Operand);
				}
				if (Arguments.Length != 0)
				{
					ret.Append(";");
				}
			}
			for (int i = 0; i < Arguments.Length; i++)
			{
				if (i != 0)
				{
					ret.Append(",");
				}
				ret.Append(Arguments[i]);
			}
			ret.Append(")");
			return ret.ToString();
		}

		public ILASTExpression Clone()
		{
			return (ILASTExpression)MemberwiseClone();
		}
	}
}
