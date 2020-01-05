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
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Messaging;

using bsn.ModuleStore.Sql.Script;

namespace bsn.ModuleStore.Mapper.AssemblyMetadata {
	internal abstract class SqlCallParameterBase {
		private static readonly Dictionary<string, SqlDbType> knownDbTypes = GetKnownDbTypes();

		private static Dictionary<string, SqlDbType> GetKnownDbTypes() {
			var result = Enum.GetValues(typeof(SqlDbType)).Cast<SqlDbType>().ToDictionary(x => x.ToString(), x => x, StringComparer.OrdinalIgnoreCase);
			result.Remove(SqlDbType.Udt.ToString());
			result.Remove(SqlDbType.Variant.ToString());
			result.Remove(SqlDbType.Structured.ToString());
			result.Add(@"sysname", SqlDbType.NVarChar);
			result.Add(@"sql_variant", SqlDbType.Variant);
			return result;
		}

		private readonly ParameterDirection direction;
		private readonly bool nullable;
		private readonly string parameterName;
		private readonly int size;
		private readonly SqlDbType sqlType;

		protected SqlCallParameterBase(ProcedureName procedureName, ProcedureParameter parameter, ParameterDirection direction, bool nullable, bool isEnumerable) {
			if (parameter == null) {
				throw new ArgumentNullException(nameof(parameter));
			}
			parameterName = parameter.ParameterName.Value;
			if (direction == ParameterDirection.ReturnValue) {
				throw new ArgumentOutOfRangeException(nameof(direction));
			}
			this.direction = direction;
			if (parameter.ParameterTypeName.IsQualified || (!knownDbTypes.TryGetValue(parameter.ParameterTypeName.Name.Value, out sqlType))) {
				sqlType = isEnumerable ? SqlDbType.Structured : SqlDbType.Udt;
			} else {
				if (parameter.ParameterTypeName.Name is TypeNameWithPrecision typeNameEx) {
					switch (sqlType) {
					case SqlDbType.Binary:
					case SqlDbType.VarBinary:
					case SqlDbType.Char:
					case SqlDbType.VarChar:
					case SqlDbType.NChar:
					case SqlDbType.NVarChar:
						size = (int)typeNameEx.Precision;
						break;
					}
				}
			}
			this.nullable = nullable;
			if (direction == ParameterDirection.Input) {
				if (parameter.Output) {
					throw new InvalidOperationException($"The {parameterName} OUTPUT argument requires an out parameter in {procedureName}");
				}
			} else {
				if (!parameter.Output) {
					throw new InvalidOperationException($"An out parameter requires argument {parameterName} to be OUTPUT in {procedureName}");
				}
				if (sqlType == SqlDbType.Structured) {
					throw new NotSupportedException($"Table valued argument {parameterName} must be READONLY in {procedureName}");
				}
			}
		}

		public ParameterDirection Direction => direction;

		public bool Nullable => nullable;

		public int Size => size;

		public SqlDbType SqlType => sqlType;

		public SqlParameter GetSqlParameter(SqlCommand command, IMethodCallMessage mcm, SqlParameter[] outArgs, IList<IDisposable> disposeList) {
			var parameter = command.CreateParameter();
			parameter.ParameterName = parameterName;
			parameter.IsNullable = nullable;
			parameter.Direction = direction;
			if (size > 0) {
				parameter.Size = size;
			}
			parameter.SqlDbType = sqlType;
			parameter.Value = SetParameterValue(mcm, disposeList) ?? DBNull.Value;
			if (Direction != ParameterDirection.Input) {
				outArgs[GetOutArgIndex()] = parameter;
			}
			return parameter;
		}

		protected virtual int GetOutArgIndex() {
			throw new NotSupportedException($"The {GetType().Name} type does not support output parameters");
		}

		protected abstract object SetParameterValue(IMethodCallMessage mcm, IList<IDisposable> disposables);
	}
}
