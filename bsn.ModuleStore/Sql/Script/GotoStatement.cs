using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class GotoStatement: SqlStatement {
		private readonly LabelName labelName;

		[Rule("<GotoStatement> ::= GOTO <LabelName>", ConstructorParameterMapping = new[] {1})]
		public GotoStatement(LabelName labelName) {
			if (labelName == null) {
				throw new ArgumentNullException("labelName");
			}
			this.labelName = labelName;
		}

		public override void WriteTo(System.IO.TextWriter writer) {
			writer.Write("GOTO ");
			labelName.WriteTo(writer);
		}
	}
}
