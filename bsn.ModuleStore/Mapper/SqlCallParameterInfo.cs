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
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

using bsn.ModuleStore.Sql.Script;

namespace bsn.ModuleStore.Mapper {
	internal class SqlCallParameterInfo: SqlCallParameterBase {
		private static bool GetParameterNullable(ParameterInfo param) {
			Type parameterType = param.ParameterType;
			if (parameterType.IsByRef) {
				parameterType = parameterType.GetElementType();
				Debug.Assert(parameterType != null);
			}
#warning Maybe implement a non-null attribute on the parameter?
			return (!parameterType.IsValueType) || (System.Nullable.GetUnderlyingType(parameterType) != null);
		}

		private static ParameterDirection GetParameterDirection(ParameterInfo param) {
			if (param == null) {
				throw new ArgumentNullException("param");
			}
			if (param.ParameterType.IsByRef) {
				if (param.IsOut) {
					return ParameterDirection.Output;
				}
				return ParameterDirection.InputOutput;
			}
			return ParameterDirection.Input;
		}

		private readonly int outArgIndex;
		private readonly ParameterInfo parameterInfo;

		public SqlCallParameterInfo(ParameterInfo param, ProcedureParameter script): base(script, GetParameterDirection(param), GetParameterNullable(param)) {
			parameterInfo = param;
#warning Maybe implement more type compatibility checks for arguments here
		}

		public string ParameterName {
			get {
				return parameterInfo.Name;
			}
		}

		protected override int GetOutArgIndex() {
			return parameterInfo.Position;
		}

		protected override object SetParameterValue(IMethodCallMessage mcm, IList<IDisposable> disposeList) {
			object value = mcm.GetArg(parameterInfo.Position);
			if (value != null) {
				switch (SqlType) {
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
			return value;
		}
	}
}
