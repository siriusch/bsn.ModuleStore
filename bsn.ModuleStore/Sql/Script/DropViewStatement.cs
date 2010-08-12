using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class DropViewStatement: DropStatement {
		private readonly ViewName viewName;

		[Rule("<DropViewStatement> ::= DROP VIEW <ViewName>", ConstructorParameterMapping = new[] {2})]
		public DropViewStatement(ViewName viewName) {
			Debug.Assert(viewName != null);
			this.viewName = viewName;
		}

		public ViewName ViewName {
			get {
				return viewName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("DROP VIEW ");
			writer.WriteScript(viewName, WhitespacePadding.None);
		}
	}
}