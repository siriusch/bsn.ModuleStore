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
		private static readonly Dictionary<string, SqlDbType> knownDbTypes = Enum.GetValues(typeof(SqlDbType)).Cast<SqlDbType>().ToDictionary(x => x.ToString(), x => x, StringComparer.OrdinalIgnoreCase);

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
			if (sqlType == SqlDbType.Xml) {
				if (value != null) {
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
				}
				parameter.SqlDbType = SqlDbType.Xml;
			} else {
				parameter.SqlDbType = sqlType;
			}
			parameter.Value = value ?? DBNull.Value;
			if (direction != ParameterDirection.Input) {
				outArgs[outArgIndex] = new KeyValuePair<SqlParameter, Type>(parameter, Nullable.GetUnderlyingType(parameterType));
			}
			return parameter;
		}
	}
}