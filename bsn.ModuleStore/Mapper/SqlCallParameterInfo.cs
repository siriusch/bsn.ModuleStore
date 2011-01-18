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
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

using bsn.ModuleStore.Sql.Script;

namespace bsn.ModuleStore.Mapper {
	internal class SqlCallParameterInfo {
		private static readonly Dictionary<string, SqlDbType> knownDbTypes = GetKnownDbTypes();

		private static Dictionary<string, SqlDbType> GetKnownDbTypes() {
			Dictionary<string, SqlDbType> result = Enum.GetValues(typeof(SqlDbType)).Cast<SqlDbType>().ToDictionary(x => x.ToString(), x => x, StringComparer.OrdinalIgnoreCase);
			result.Add("sysname", SqlDbType.NVarChar);
			return result;
		}

		private readonly string argName;
		private readonly ParameterDirection direction = ParameterDirection.Input;
		private readonly bool nullable;
		private readonly int outArgIndex;
		private readonly Type parameterType;
		private readonly int size;
		private readonly SqlDbType sqlType;

		public SqlCallParameterInfo(ParameterInfo param, ProcedureParameter script, ref bool hasOutArg) {
			parameterType = param.ParameterType;
			if (parameterType.IsByRef) {
				if (!script.Output) {
					throw new InvalidOperationException("An out parameter requires an OUTPUT argument");
				}
				parameterType = parameterType.GetElementType();
				outArgIndex = param.Position;
				hasOutArg = true;
				if (param.IsOut) {
					direction = ParameterDirection.Output;
				} else {
					direction = ParameterDirection.InputOutput;
				}
				Debug.Assert(parameterType != null);
			} else if (script.Output) {
				throw new InvalidOperationException("An OUTPUT argument requires an out parameter");
			}
			argName = script.ParameterName.Value;
			if (script.ParameterTypeName.IsQualified || (!knownDbTypes.TryGetValue(script.ParameterTypeName.Name.Value, out sqlType))) {
				sqlType = SqlDbType.Udt;
				Debug.Fail("UDT?");
			} else {
				TypeNameWithPrecision typeNameEx = script.ParameterTypeName.Name as TypeNameWithPrecision;
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
			nullable = (!parameterType.IsValueType) || (Nullable.GetUnderlyingType(parameterType) != null);
#warning Maybe implement more type compatibility checks for arguments here
		}

		public SqlParameter GetSqlParameter(SqlCommand command, object value, KeyValuePair<SqlParameter, Type>[] outArgs, IList<IDisposable> disposeList) {
			SqlParameter parameter = command.CreateParameter();
			parameter.ParameterName = argName;
			parameter.IsNullable = nullable;
			parameter.Direction = direction;
			if (size > 0) {
				parameter.Size = size;
			}
			parameter.SqlDbType = sqlType;
			if (value != null) {
				switch (sqlType) {
				case SqlDbType.Xml:
					XmlReader reader = value as XmlReader;
					if (reader == null) {
						XPathNavigator navigator = value as XPathNavigator;
						if (navigator == null) {
							IXPathNavigable navigable = value as IXPathNavigable;
							if (navigable != null) {
								navigator = navigable.CreateNavigator();
							} else {
								XNode node = value as XNode;
								if (node != null) {
									navigator = node.CreateNavigator();
								}
							}
							if (navigator == null) {
								throw new NotSupportedException(String.Format("XML could not be retrieved from value of type {0}.", value.GetType()));
							}
						}
						reader = navigator.ReadSubtree();
						disposeList.Add(reader);
					}
					value = new SqlXml(reader);
					break;
				case SqlDbType.SmallInt:
					IIdentifiable<short> identifiableShort = value as IIdentifiable<short>;
					if (identifiableShort != null) {
						value = identifiableShort.Id;
					}
					break;
				case SqlDbType.Int:
					IIdentifiable<int> identifiableInt = value as IIdentifiable<int>;
					if (identifiableInt != null) {
						value = identifiableInt.Id;
					}
					break;
				case SqlDbType.BigInt:
					IIdentifiable<long> identifiableLong = value as IIdentifiable<long>;
					if (identifiableLong != null) {
						value = identifiableLong.Id;
					}
					break;
				case SqlDbType.UniqueIdentifier:
					IIdentifiable<Guid> identifiableGuid = value as IIdentifiable<Guid>;
					if (identifiableGuid != null) {
						value = identifiableGuid.Id;
					}
					break;
				}
			}
			parameter.Value = value ?? DBNull.Value;
			if (direction != ParameterDirection.Input) {
				outArgs[outArgIndex] = new KeyValuePair<SqlParameter, Type>(parameter, Nullable.GetUnderlyingType(parameterType));
			}
			return parameter;
		}
	}
}
