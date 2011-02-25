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
//  
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;

using bsn.ModuleStore.Sql.Script;

namespace bsn.ModuleStore.Mapper {
	internal abstract class SqlCallParameterBase {
		private static readonly Dictionary<string, SqlDbType> knownDbTypes = GetKnownDbTypes();

		private static Dictionary<string, SqlDbType> GetKnownDbTypes() {
			Dictionary<string, SqlDbType> result = Enum.GetValues(typeof(SqlDbType)).Cast<SqlDbType>().ToDictionary(x => x.ToString(), x => x, StringComparer.OrdinalIgnoreCase);
			result.Add(@"sysname", SqlDbType.NVarChar);
			return result;
		}

		private readonly ParameterDirection direction;
		private readonly bool nullable;

		private readonly string parameterName;
		private readonly int size;
		private readonly SqlDbType sqlType;

		protected SqlCallParameterBase(ProcedureParameter parameter, ParameterDirection direction, bool nullable) {
			if (parameter == null) {
				throw new ArgumentNullException("parameter");
			}
			parameterName = parameter.ParameterName.Value;
			if (direction == ParameterDirection.ReturnValue) {
				throw new ArgumentOutOfRangeException("direction");
			}
			if (direction == ParameterDirection.Input) {
				if (parameter.Output) {
					throw new InvalidOperationException("An OUTPUT argument requires an out parameter");
				}
			} else {
				if (!parameter.Output) {
					throw new InvalidOperationException("An out parameter requires an OUTPUT argument");
				}
			}
			this.direction = direction;
			if (parameter.ParameterTypeName.IsQualified || (!knownDbTypes.TryGetValue(parameter.ParameterTypeName.Name.Value, out sqlType))) {
				sqlType = SqlDbType.Udt;
				Debug.Fail("UDT?");
			} else {
				TypeNameWithPrecision typeNameEx = parameter.ParameterTypeName.Name as TypeNameWithPrecision;
				if (typeNameEx != null) {
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
		}

		public ParameterDirection Direction {
			get {
				return direction;
			}
		}

		public bool Nullable {
			get {
				return nullable;
			}
		}

		public int Size {
			get {
				return size;
			}
		}

		public SqlDbType SqlType {
			get {
				return sqlType;
			}
		}

		public SqlParameter GetSqlParameter(SqlCommand command, IMethodCallMessage mcm, SqlParameter[] outArgs, IList<IDisposable> disposeList) {
			SqlParameter parameter = command.CreateParameter();
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
			throw new NotSupportedException(string.Format("The {0} type does not support output parameters", GetType().Name));
		}

		protected abstract object SetParameterValue(IMethodCallMessage mcm, IList<IDisposable> disposables);
	}
}
