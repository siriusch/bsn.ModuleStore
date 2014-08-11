using System;
using System.Drawing;

using bsn.GoldParser.Text;
using bsn.ModuleStore.Sql;

namespace bsn.ModuleStore.Console {
	internal class SqlStyleProvider: IStyleProvider {
		public void Reset(RichTextWriter writer) {
			writer.Reset();
		}

		public void Set<T>(RichTextWriter writer, T style) where T: struct, IComparable, IFormattable, IConvertible {
			if (typeof(T) == typeof(SqlTextKind)) {
				switch ((SqlTextKind)(object)style) {
				case SqlTextKind.Comment:
					writer.SetForeground(Color.Green);
					break;
				case SqlTextKind.Identifier:
					writer.SetForeground(Color.DarkCyan);
					break;
				case SqlTextKind.Keyword:
					writer.SetForeground(Color.White);
					break;
				case SqlTextKind.Literal:
					writer.SetForeground(Color.Yellow);
					break;
				case SqlTextKind.Type:
					writer.SetForeground(Color.Blue);
					break;
				case SqlTextKind.Function:
					writer.SetForeground(Color.Magenta);
					break;
				case SqlTextKind.Operator:
					writer.SetForeground(Color.DarkMagenta);
					break;
				case SqlTextKind.String:
					writer.SetForeground(Color.Red);
					break;
				default:
					writer.Reset();
					break;
				}
			}
		}
	}
}
