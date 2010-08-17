using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script;

namespace bsn.ModuleStore.Sql {
	internal class SemanticSqlProcessor: SemanticProcessor<SqlToken> {
		private readonly List<IQualifiedName<SchemaName>> schemaBoundNames = new List<IQualifiedName<SchemaName>>();

		public SemanticSqlProcessor(TextReader reader, SemanticActions<SqlToken> actions): base(reader, actions) {
		}

		protected override SemanticToken CreateReduction(bsn.GoldParser.Grammar.Rule rule, System.Collections.ObjectModel.ReadOnlyCollection<SemanticToken> children) {
			SemanticToken result = base.CreateReduction(rule, children);
			IQualifiedName<SchemaName> schemaBoundName = result as IQualifiedName<SchemaName>;
			if (schemaBoundName != null) {
				schemaBoundNames.Add(schemaBoundName);
			}
			return result;
		}

		public List<IQualifiedName<SchemaName>> SchemaBoundNames {
			get {
				return schemaBoundNames;
			}
		}
	}
}
