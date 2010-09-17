using System;
using System.Diagnostics;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class GotoStatement: Statement {
		private readonly LabelName labelName;

		[Rule("<GotoStatement> ::= ~GOTO <LabelName>")]
		public GotoStatement(LabelName labelName) {
			Debug.Assert(labelName != null);
			this.labelName = labelName;
		}

		public LabelName LabelName {
			get {
				return labelName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			writer.Write("GOTO ");
			writer.WriteScript(labelName, WhitespacePadding.None);
		}
	}
}