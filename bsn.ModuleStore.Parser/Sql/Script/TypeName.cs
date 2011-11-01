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

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class TypeName: SqlQuotedName {
		private static KeyValuePair<string, bool> FormatName(string name) {
			if (string.IsNullOrEmpty(name)) {
				throw new ArgumentNullException("name");
			}
			bool isBuiltIn = SqlTypeMapping.TryGetBuiltinTypeName(ref name);
			return new KeyValuePair<string, bool>(name, isBuiltIn);
		}

		private readonly bool builtinType;

		[Rule("<SimpleTypeName> ::= Id")]
		[Rule("<SimpleTypeName> ::= QuotedId")]
		public TypeName(SqlIdentifier identifier): this(identifier.Value) {}

		internal TypeName(string name): this(FormatName(name)) {}

		private TypeName(KeyValuePair<string, bool> typeName): base(typeName.Key) {
			builtinType = typeName.Value;
		}

		public bool IsBuiltinType {
			get {
				return builtinType;
			}
		}

		protected internal override void WriteToInternal(SqlWriter writer, bool isPartOfQualifiedName) {
			if (isPartOfQualifiedName || (!IsBuiltinType)) {
				base.WriteToInternal(writer, isPartOfQualifiedName);
			} else {
				writer.Write(Value);
			}
		}
	}
}
