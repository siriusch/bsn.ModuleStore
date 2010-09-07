using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace bsn.ModuleStore.Mapper {
	internal class SqlCallParameterInfo {
		private readonly Type parameterType;
		private readonly SqlArgAttribute arg;
		private readonly ParameterDirection direction = ParameterDirection.Input;
		private readonly int outArgIndex;

		public SqlCallParameterInfo(ParameterInfo param, ref bool hasOutArg) {
			parameterType = param.ParameterType;
			if (parameterType.IsByRef) {
				parameterType = parameterType.GetElementType();
			}
			foreach (SqlArgAttribute attribute in param.GetCustomAttributes(typeof(SqlArgAttribute), false)) {
				arg = attribute;
			}
			if (arg == null) {
				arg = new SqlArgAttribute(param.Name);
			} else if (String.IsNullOrEmpty(arg.Name)) {
				arg = (SqlArgAttribute)arg.CloneWithName(param.Name);
			}
			if (!arg.HasType) {
				bool? nullable;
				arg.Type = SqlArgAttribute.GetTypeMapping(param.ParameterType, out nullable);
				if ((arg.Size == 0) && ((arg.Type == SqlDbType.NChar) || (arg.Type == SqlDbType.Char))) {
					arg.Size = 1;
				}
				if (nullable.HasValue) {
					arg.Nullable = nullable.Value;
				}
			}
			if (param.ParameterType.IsByRef) {
				outArgIndex = param.Position;
				hasOutArg = true;
				if (param.IsOut) {
					direction = ParameterDirection.Output;
				} else {
					direction = ParameterDirection.InputOutput;
				}
			}
		}

		public SqlParameter GetSqlParameter(SqlCommand command, object value, KeyValuePair<SqlParameter, Type>[] outArgs, IList<IDisposable> disposeList) {
			SqlParameter parameter = command.CreateParameter();
			parameter.ParameterName = arg.Name;
			if (arg.Nullable) {
				parameter.IsNullable = true;
			}
			if (arg.Size > 0) {
				parameter.Size = arg.Size;
			}
			parameter.Direction = direction;
			if (arg.Type == SqlDbType.Xml) {
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
				parameter.SqlDbType = arg.Type;
			}
			parameter.Value = value ?? DBNull.Value;
			if (direction != ParameterDirection.Input) {
				Type conversion = null;
				if (SqlDeserializer.IsNullableType(parameterType)) {
					conversion = parameterType;
				}
				outArgs[outArgIndex] = new KeyValuePair<SqlParameter, Type>(parameter, conversion);
			}
			return parameter;
		}
	}
}