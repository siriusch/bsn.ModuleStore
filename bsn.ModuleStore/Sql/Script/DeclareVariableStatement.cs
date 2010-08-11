using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class DeclareVariableStatement: Statement {
		private readonly List<DeclareItem> declarations;

		[Rule("<DeclareStatement> ::= DECLARE <DeclareItemList>", ConstructorParameterMapping = new[] {1})]
		public DeclareVariableStatement(Sequence<DeclareItem> declarations) {
			if (declarations == null) {
				throw new ArgumentNullException("declarations");
			}
			this.declarations = declarations.ToList();
		}

		public List<DeclareItem> Declarations {
			get {
				return declarations;
			}
		}

		public override void WriteTo(TextWriter writer) {
			writer.Write("DECLARE ");
			writer.WriteSequence(declarations, null, ", ", null);
		}
	}
}