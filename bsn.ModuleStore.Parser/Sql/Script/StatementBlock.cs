using System;
using System.Collections.Generic;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class StatementBlock: Statement {
		private readonly List<Statement> statements;

		[Rule("<StatementBlock> ::= ~BEGIN <StatementList> ~END")]
		public StatementBlock(Sequence<Statement> statements): this(statements.ToList()) {}

		internal StatementBlock(params Statement[] statements): this(new List<Statement>(statements)) {}

		private StatementBlock(List<Statement> statements) {
			Debug.Assert(statements != null);
			this.statements = statements;
			if (this.statements.Count == 1) {
				StatementBlock innerBlock = this.statements[0] as StatementBlock;
				if (innerBlock != null) {
					this.statements = innerBlock.statements;
				}
			}
		}

		public IEnumerable<Statement> Statements {
			get {
				return statements;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("BEGIN");
			writer.IncreaseIndent();
			writer.WriteScriptSequence(statements, WhitespacePadding.NewlineBefore, ";");
			writer.DecreaseIndent();
			writer.WriteLine(";");
			writer.Write("END");
		}
	}
}
