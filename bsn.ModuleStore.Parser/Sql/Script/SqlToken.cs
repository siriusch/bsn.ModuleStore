using System;
using System.Collections.Generic;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class SqlToken: SemanticToken {
		internal IEnumerable<IQualifiedName<SchemaName>> GetInnerSchemaQualifiedNames(Predicate<string> checkSchemaName) {
			return GetInnerTokens(delegate(IQualifiedName<SchemaName> name, CommonTableExpressionScope scope) {
			                      	if (name.IsQualified) {
			                      		return checkSchemaName(name.Qualification.Value);
			                      	}
			                      	return (name.Name is TableName) && (!scope.ContainsName(name.Name.Value));
			                      });
		}

		internal IEnumerable<T> GetInnerTokens<T>(Func<T, CommonTableExpressionScope, bool> predicate) where T: class {
			Queue<KeyValuePair<SqlToken, CommonTableExpressionScope>> itemsToProcess = new Queue<KeyValuePair<SqlToken, CommonTableExpressionScope>>();
			itemsToProcess.Enqueue(new KeyValuePair<SqlToken, CommonTableExpressionScope>(this, new CommonTableExpressionScope(null)));
			do {
				KeyValuePair<SqlToken, CommonTableExpressionScope> pair = itemsToProcess.Dequeue();
				Debug.Assert(pair.Key != null);
				SqlTokenMetadataFactory.SqlTokenMetadata tokenMetadata = SqlTokenMetadataFactory.GetTokenMetadataInternal(pair.Key.GetType(), null);
				CommonTableExpressionScope scope = tokenMetadata.IsCommonTableExpressionScope ? new CommonTableExpressionScope(pair.Value) : pair.Value;
				foreach (SqlToken innerToken in tokenMetadata.EnumerateTokensUntyped(pair.Key)) {
					CommonTableExpression cte = innerToken as CommonTableExpression;
					if (cte != null) {
						scope.Names.Add(cte.AliasName.Value);
					}
					T typedInnerToken = innerToken as T;
					if (typedInnerToken != null) {
						if ((predicate == null) || predicate(typedInnerToken, scope)) {
							yield return typedInnerToken;
						}
					}
					itemsToProcess.Enqueue(new KeyValuePair<SqlToken, CommonTableExpressionScope>(innerToken, scope));
				}
			} while (itemsToProcess.Count > 0);
		}
	}

	internal class CommonTableExpressionScope {
		private readonly HashSet<string> names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		private readonly CommonTableExpressionScope parent;

		public CommonTableExpressionScope(CommonTableExpressionScope parent) {
			this.parent = parent;
		}

		public HashSet<string> Names {
			get {
				return names;
			}
		}

		public CommonTableExpressionScope Parent {
			get {
				return parent;
			}
		}

		public bool ContainsName(string name) {
			if (names.Contains(name)) {
				return true;
			}
			if (parent != null) {
				return parent.ContainsName(name);
			}
			return false;
		}
	}
}
