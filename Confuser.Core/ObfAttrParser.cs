using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confuser.Core
{
	// Token: 0x0200004C RID: 76
	internal struct ObfAttrParser
	{
		// Token: 0x060001C7 RID: 455 RVA: 0x0000ED80 File Offset: 0x0000CF80
		public ObfAttrParser(IDictionary items)
		{
			this.items = items;
			this.str = null;
			this.index = -1;
		}

		// Token: 0x060001C8 RID: 456 RVA: 0x0000ED98 File Offset: 0x0000CF98
		private bool ReadId(StringBuilder sb)
		{
			while (this.index < this.str.Length)
			{
				char c = this.str[this.index];
				switch (c)
				{
				case '(':
				case ')':
				case '+':
				case ',':
				case '-':
					return true;
				case '*':
					break;
				default:
					switch (c)
					{
					case ';':
					case '=':
						return true;
					}
					break;
				}
				sb.Append(this.str[this.index++]);
			}
			return false;
		}

		// Token: 0x060001C9 RID: 457 RVA: 0x0000EE30 File Offset: 0x0000D030
		private bool ReadString(StringBuilder sb)
		{
			this.Expect('\'');
			while (this.index < this.str.Length)
			{
				char c = this.str[this.index];
				if (c == '\'')
				{
					this.index++;
					return true;
				}
				if (c == '\\')
				{
					sb.Append(this.str[++this.index]);
				}
				else
				{
					sb.Append(this.str[this.index]);
				}
				this.index++;
			}
			return false;
		}

		// Token: 0x060001CA RID: 458 RVA: 0x0000EED8 File Offset: 0x0000D0D8
		private void Expect(char chr)
		{
			if (this.str[this.index] != chr)
			{
				throw new ArgumentException(string.Concat(new object[]
				{
					"Expect '",
					chr,
					"' at position ",
					this.index + 1,
					"."
				}));
			}
			this.index++;
		}

		// Token: 0x060001CB RID: 459 RVA: 0x0000EF4B File Offset: 0x0000D14B
		private char Peek()
		{
			return this.str[this.index];
		}

		// Token: 0x060001CC RID: 460 RVA: 0x0000EF5E File Offset: 0x0000D15E
		private void Next()
		{
			this.index++;
		}

		// Token: 0x060001CD RID: 461 RVA: 0x0000EF6E File Offset: 0x0000D16E
		private bool IsEnd()
		{
			return this.index == this.str.Length;
		}

		// Token: 0x060001CE RID: 462 RVA: 0x0000EFA0 File Offset: 0x0000D1A0
		public void ParseProtectionString(ProtectionSettings settings, string str)
		{
			if (str == null)
			{
				return;
			}
			this.str = str;
			this.index = 0;
			ObfAttrParser.ParseState state = ObfAttrParser.ParseState.Init;
			StringBuilder buffer = new StringBuilder();
			bool protAct = true;
			string protId = null;
			Dictionary<string, string> protParams = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			while (state != ObfAttrParser.ParseState.End)
			{
				switch (state)
				{
				case ObfAttrParser.ParseState.Init:
					this.ReadId(buffer);
					if (buffer.ToString().Equals("preset", StringComparison.OrdinalIgnoreCase))
					{
						if (this.IsEnd())
						{
							throw new ArgumentException("Unexpected end of string in Init state.");
						}
						this.Expect('(');
						buffer.Length = 0;
						state = ObfAttrParser.ParseState.ReadPreset;
					}
					else if (buffer.Length == 0)
					{
						if (this.IsEnd())
						{
							throw new ArgumentException("Unexpected end of string in Init state.");
						}
						state = ObfAttrParser.ParseState.ReadItemName;
					}
					else
					{
						protAct = true;
						state = ObfAttrParser.ParseState.ProcessItemName;
					}
					break;
				case ObfAttrParser.ParseState.ReadPreset:
				{
					if (!this.ReadId(buffer))
					{
						throw new ArgumentException("Unexpected end of string in ReadPreset state.");
					}
					this.Expect(')');
					ProtectionPreset preset = (ProtectionPreset)Enum.Parse(typeof(ProtectionPreset), buffer.ToString(), true);
					foreach (Protection item in from prot in this.items.Values.OfType<Protection>()
					where prot.Preset <= preset
					select prot)
					{
						if (settings != null && !settings.ContainsKey(item))
						{
							settings.Add(item, new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
						}
					}
					buffer.Length = 0;
					if (this.IsEnd())
					{
						state = ObfAttrParser.ParseState.End;
					}
					else
					{
						this.Expect(';');
						if (this.IsEnd())
						{
							state = ObfAttrParser.ParseState.End;
						}
						else
						{
							state = ObfAttrParser.ParseState.ReadItemName;
						}
					}
					break;
				}
				case ObfAttrParser.ParseState.ReadItemName:
					protAct = true;
					if (this.Peek() == '+')
					{
						protAct = true;
						this.Next();
					}
					else if (this.Peek() == '-')
					{
						protAct = false;
						this.Next();
					}
					this.ReadId(buffer);
					state = ObfAttrParser.ParseState.ProcessItemName;
					break;
				case ObfAttrParser.ParseState.ProcessItemName:
					protId = buffer.ToString();
					buffer.Length = 0;
					if (this.IsEnd() || this.Peek() == ';')
					{
						state = ObfAttrParser.ParseState.EndItem;
					}
					else
					{
						if (this.Peek() != '(')
						{
							throw new ArgumentException("Unexpected character in ProcessItemName state at " + this.index + ".");
						}
						if (!protAct)
						{
							throw new ArgumentException("No parameters is allowed when removing protection.");
						}
						this.Next();
						state = ObfAttrParser.ParseState.ReadParam;
					}
					break;
				case ObfAttrParser.ParseState.ReadParam:
				{
					if (!this.ReadId(buffer))
					{
						throw new ArgumentException("Unexpected end of string in ReadParam state.");
					}
					string paramName = buffer.ToString();
					buffer.Length = 0;
					this.Expect('=');
					if (!((this.Peek() == '\'') ? this.ReadString(buffer) : this.ReadId(buffer)))
					{
						throw new ArgumentException("Unexpected end of string in ReadParam state.");
					}
					string paramValue = buffer.ToString();
					buffer.Length = 0;
					protParams.Add(paramName, paramValue);
					if (this.Peek() == ',')
					{
						this.Next();
						state = ObfAttrParser.ParseState.ReadParam;
					}
					else
					{
						if (this.Peek() != ')')
						{
							throw new ArgumentException("Unexpected character in ReadParam state at " + this.index + ".");
						}
						this.Next();
						state = ObfAttrParser.ParseState.EndItem;
					}
					break;
				}
				case ObfAttrParser.ParseState.EndItem:
					if (settings != null)
					{
						if (!this.items.Contains(protId))
						{
							throw new KeyNotFoundException("Cannot find protection with id '" + protId + "'.");
						}
						if (protAct)
						{
							settings[(Protection)this.items[protId]] = protParams;
						}
						else
						{
							settings.Remove((Protection)this.items[protId]);
						}
					}
					protParams = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
					if (this.IsEnd())
					{
						state = ObfAttrParser.ParseState.End;
					}
					else
					{
						this.Expect(';');
						if (this.IsEnd())
						{
							state = ObfAttrParser.ParseState.End;
						}
						else
						{
							state = ObfAttrParser.ParseState.ReadItemName;
						}
					}
					break;
				}
			}
		}

		// Token: 0x060001CF RID: 463 RVA: 0x0000F354 File Offset: 0x0000D554
		public void ParsePackerString(string str, out Packer packer, out Dictionary<string, string> packerParams)
		{
			packer = null;
			packerParams = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			if (str == null)
			{
				return;
			}
			this.str = str;
			this.index = 0;
			ObfAttrParser.ParseState state = ObfAttrParser.ParseState.ReadItemName;
			StringBuilder buffer = new StringBuilder();
			new ProtectionSettings();
			while (state != ObfAttrParser.ParseState.End)
			{
				switch (state)
				{
				case ObfAttrParser.ParseState.ReadItemName:
				{
					this.ReadId(buffer);
					string packerId = buffer.ToString();
					if (!this.items.Contains(packerId))
					{
						throw new KeyNotFoundException("Cannot find packer with id '" + packerId + "'.");
					}
					packer = (Packer)this.items[packerId];
					buffer.Length = 0;
					if (this.IsEnd() || this.Peek() == ';')
					{
						state = ObfAttrParser.ParseState.EndItem;
					}
					else
					{
						if (this.Peek() != '(')
						{
							throw new ArgumentException("Unexpected character in ReadItemName state at " + this.index + ".");
						}
						this.Next();
						state = ObfAttrParser.ParseState.ReadParam;
					}
					break;
				}
				case ObfAttrParser.ParseState.ReadParam:
				{
					if (!this.ReadId(buffer))
					{
						throw new ArgumentException("Unexpected end of string in ReadParam state.");
					}
					string paramName = buffer.ToString();
					buffer.Length = 0;
					this.Expect('=');
					if (!this.ReadId(buffer))
					{
						throw new ArgumentException("Unexpected end of string in ReadParam state.");
					}
					string paramValue = buffer.ToString();
					buffer.Length = 0;
					packerParams.Add(paramName, paramValue);
					if (this.Peek() == ',')
					{
						this.Next();
						state = ObfAttrParser.ParseState.ReadParam;
					}
					else
					{
						if (this.Peek() != ')')
						{
							throw new ArgumentException("Unexpected character in ReadParam state at " + this.index + ".");
						}
						this.Next();
						state = ObfAttrParser.ParseState.EndItem;
					}
					break;
				}
				case ObfAttrParser.ParseState.EndItem:
					if (this.IsEnd())
					{
						state = ObfAttrParser.ParseState.End;
					}
					else
					{
						this.Expect(';');
						if (!this.IsEnd())
						{
							throw new ArgumentException("Unexpected character in EndItem state at " + this.index + ".");
						}
						state = ObfAttrParser.ParseState.End;
					}
					break;
				}
			}
		}

		// Token: 0x04000154 RID: 340
		private readonly IDictionary items;

		// Token: 0x04000155 RID: 341
		private string str;

		// Token: 0x04000156 RID: 342
		private int index;

		// Token: 0x0200004D RID: 77
		private enum ParseState
		{
			// Token: 0x04000158 RID: 344
			Init,
			// Token: 0x04000159 RID: 345
			ReadPreset,
			// Token: 0x0400015A RID: 346
			ReadItemName,
			// Token: 0x0400015B RID: 347
			ProcessItemName,
			// Token: 0x0400015C RID: 348
			ReadParam,
			// Token: 0x0400015D RID: 349
			EndItem,
			// Token: 0x0400015E RID: 350
			End
		}
	}
}
