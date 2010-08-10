using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class DropViewStatement: DropStatement {
		private readonly ViewName viewName;

		[Rule("<DropViewStatement> ::= DROP VIEW <ViewName>", ConstructorParameterMapping = new[] {2})]
		public DropViewStatement(ViewName viewName) {
			if (viewName == null) {
				throw new ArgumentNullException("viewName");
			}
			this.viewName = viewName;
		}

		public ViewName ViewName {
			get {
				return viewName;
			}
		}

		public override void WriteTo(TextWriter writer) {
			writer.Write("DROP VIEW ");
			writer.WriteScript(viewName);
		}
	}
}