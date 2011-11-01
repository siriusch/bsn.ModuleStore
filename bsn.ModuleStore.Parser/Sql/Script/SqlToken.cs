// bsn ModuleStore database versioning
// -----------------------------------
// 
// Copyright 2010 by Arsène von Wyss - avw@gmx.ch
// 
// Development has been supported by Sirius Technologies AG, Basel
// 
// Source:
// 
// https://bsn-modulestore.googlecode.com/hg/
// 
// License:
// 
// The library is distributed under the GNU Lesser General Public License:
// http://www.gnu.org/licenses/lgpl.html
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class SqlToken: SemanticToken {
		private static readonly Regex rxInsertedDeletedTables = new Regex("^(INSERTED|DELETED)$", RegexOptions.CultureInvariant|RegexOptions.IgnoreCase|RegexOptions.ExplicitCapture);

		internal static bool IsInsertedOrDeletedTableName(string tableName) {
			return rxInsertedDeletedTables.IsMatch(tableName);
		}

		internal IEnumerable<IQualifiedName<SchemaName>> GetInnerSchemaQualifiedNames(Predicate<string> checkSchemaName) {
			return GetInnerTokens(delegate(IQualifiedName<SchemaName> name, CommonTableExpressionScope scope) {
				if (!name.LockedOverride) {
					SchemaName qualification = name.Qualification;
					if (qualification != null) {
						Debug.Assert(!string.IsNullOrEmpty(qualification.Value));
						return checkSchemaName(qualification.Value);
					}
					TableName tableName = name.Name as TableName;
					if (((tableName != null) || (name.Name is ViewName)) && scope.ContainsName(name.Name.Value)) {
						return false;
					}
					if (tableName != null) {
						return !tableName.Value.StartsWith("#");
					}
					TypeName typeName = name.Name as TypeName;
					if (typeName != null) {
						return !typeName.IsBuiltinType;
					}
					FunctionName functionName = name.Name as FunctionName;
					if (functionName != null) {
						return !functionName.IsBuiltinFunction;
					}
					return true;
				}
				return false;
			}, null);
		}

		internal IEnumerable<T> GetInnerTokens<T>(Func<T, CommonTableExpressionScope, bool> predicate, Type skipNestedOfType) where T: class {
			Queue<KeyValuePair<SqlToken, CommonTableExpressionScope>> itemsToProcess = new Queue<KeyValuePair<SqlToken, CommonTableExpressionScope>>();
			itemsToProcess.Enqueue(new KeyValuePair<SqlToken, CommonTableExpressionScope>(this, new CommonTableExpressionScope(null)));
			do {
				KeyValuePair<SqlToken, CommonTableExpressionScope> pair = itemsToProcess.Dequeue();
				Debug.Assert(pair.Key != null);
				SqlTokenMetadataFactory.SqlTokenMetadata tokenMetadata = SqlTokenMetadataFactory.GetTokenMetadataInternal(pair.Key.GetType(), null);
				CommonTableExpressionScope scope = tokenMetadata.IsCommonTableExpressionScope ? new CommonTableExpressionScope(pair.Value) : pair.Value;
				foreach (SqlToken innerToken in tokenMetadata.EnumerateTokensUntyped(pair.Key, skipNestedOfType)) {
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

		internal void LockInnerUnqualifiedTableNames(Predicate<string> lockPredicate) {
			if (lockPredicate == null) {
				throw new ArgumentNullException("lockPredicate");
			}
			foreach (IQualifiedName<SchemaName> schemaQualifiedName in GetInnerSchemaQualifiedNames(s => false)) {
				if (lockPredicate(schemaQualifiedName.Name.Value)) {
					schemaQualifiedName.LockOverride();
				}
			}
		}
	}
}
