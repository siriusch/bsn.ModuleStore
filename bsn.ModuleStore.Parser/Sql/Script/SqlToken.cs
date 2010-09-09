using System;
using System.Collections.Generic;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class SqlToken: SemanticToken {
		internal IEnumerable<IQualifiedName<SchemaName>> GetInnerSchemaQualifiedNames(Predicate<string> checkSchemaName) {
			return GetInnerTokens(delegate(IQualifiedName<SchemaName> name, CommonTableExpressionScope scope) {
			                      	SchemaName qualification = name.Qualification;
			                      	if (qualification != null) {
			                      		return checkSchemaName(qualification.Value);
			                      	}
			                      	if (((name.Name is TableName) || (name.Name is ViewName)) && (!scope.ContainsName(name.Name.Value))) {
			                      		return true;
			                      	}
			                      	FunctionName functionName = name.Name as FunctionName;
			                      	if ((functionName != null) && (!functionName.IsBuiltinFunction)) {
			                      		return true;
			                      	}
			                      	return false;
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
						scope.AddName(cte.AliasName.Value);
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
}
