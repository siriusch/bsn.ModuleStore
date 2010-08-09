using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CommonTableExpression: SqlToken, IScriptable {
		private readonly AliasName aliasName;
		private readonly SelectQuery selectQuery;

		[Rule("<CTE> ::= <AliasName> AS '(' <SelectQuery> ')'", ConstructorParameterMapping = new[] {0, 3})]
		public CommonTableExpression(AliasName aliasName, SelectQuery selectQuery) {
			if (aliasName == null) {
				throw new ArgumentNullException("aliasName");
			}
			if (selectQuery == null) {
				throw new ArgumentNullException("selectQuery");
			}
			this.aliasName = aliasName;
			this.selectQuery = selectQuery;
		}

		public AliasName AliasName {
			get {
				return aliasName;
			}
		}

		public SelectQuery SelectQuery {
			get {
				return selectQuery;
			}
		}

		public void WriteTo(TextWriter writer) {
			writer.WriteScript(aliasName);
			writer.WriteLine(" AS (");
			writer.WriteScript(selectQuery);
			writer.WriteLine();
			writer.Write(')');
		}
	}
}