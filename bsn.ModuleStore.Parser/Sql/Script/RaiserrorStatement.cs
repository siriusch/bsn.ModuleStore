using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class RaiserrorStatement: Statement {
		private readonly List<Expression> arguments;

		[Rule("<RaiserrorStatement> ::= ~RAISERROR ~'(' <ExpressionList> ~')'")]
		public RaiserrorStatement(Sequence<Expression> arguments) {
			Debug.Assert(arguments != null);
			this.arguments = arguments.ToList();
		}

		public IEnumerable<Expression> Arguments {
			get {
				return arguments;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			writer.Write("RAISERROR(");
			writer.WriteScriptSequence(arguments, WhitespacePadding.None, ", ");
			writer.Write(")");
		}
	}
}