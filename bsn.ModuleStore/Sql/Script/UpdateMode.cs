using System;
using System.IO;
using System.Text.RegularExpressions;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("READ_ONLY")]
	[Terminal("FOR_UPDATE")]
	public class UpdateMode: SqlToken {
		private static readonly Regex rxReadOnly = new Regex(@"READ\s+ONLY", RegexOptions.CultureInvariant|RegexOptions.IgnoreCase);

		private readonly Sequence<ColumnName> columns;
		private readonly string mode;

		public UpdateMode(string mode) {
			this.mode = rxReadOnly.IsMatch(mode) ? "READ ONLY" : "FOR UPDATE";
		}

		[Rule("<CursorUpdate> ::= FOR_UPDATE OF <ColumnNameList>", ConstructorParameterMapping = new[] {2})]
		public UpdateMode(Sequence<ColumnName> columns): this("FOR UPDATE") {
			this.columns = columns;
		}

		public override void WriteTo(TextWriter writer) {
			writer.Write(mode);
			if (columns != null) {
				string prepend = " OF ";
				foreach (ColumnName columnName in columns) {
					writer.Write(prepend);
					columnName.WriteTo(writer);
					prepend = ", ";
				}
			}
		}
	}
}