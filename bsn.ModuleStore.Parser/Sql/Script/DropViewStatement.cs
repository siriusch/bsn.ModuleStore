using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class DropViewStatement: DropStatement {
		private readonly Qualified<SchemaName, ViewName> viewName;

		[Rule("<DropViewStatement> ::= ~DROP ~VIEW <ViewNameQualified>")]
		public DropViewStatement(Qualified<SchemaName, ViewName> viewName) {
			Debug.Assert(viewName != null);
			this.viewName = viewName;
		}

		public Qualified<SchemaName, ViewName> ViewName {
			get {
				return viewName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			writer.Write("DROP VIEW ");
			writer.WriteScript(viewName, WhitespacePadding.None);
		}
	}
}