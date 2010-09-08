using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class DeclareVariableStatement: Statement {
		private readonly List<DeclareItem> declarations;

		[Rule("<DeclareStatement> ::= DECLARE <DeclareItemList>", ConstructorParameterMapping = new[] {1})]
		public DeclareVariableStatement(Sequence<DeclareItem> declarations) {
			Debug.Assert(declarations != null);
			this.declarations = declarations.ToList();
		}

		public IEnumerable<DeclareItem> Declarations {
			get {
				return declarations;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("DECLARE ");
			writer.WriteScriptSequence(declarations, WhitespacePadding.None, ", ");
		}
	}
}