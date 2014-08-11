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
using System.Globalization;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class ObjectIdFunction: FunctionCall {
		private readonly IQualifiedName<SchemaName> objectName;
		private readonly StringLiteral objectType;
		private readonly bool unicodeObjectName;

		public ObjectIdFunction(IQualifiedName<SchemaName> objectName, bool unicodeObjectName = true, StringLiteral objectType = null) {
			this.objectName = objectName;
			this.objectType = objectType;
			this.unicodeObjectName = unicodeObjectName;
		}

		[Rule("<FunctionCall> ::= ~OBJECT_ID ~'(' StringLiteral ~')'")]
		public ObjectIdFunction(StringLiteral objectName): this(objectName, null) {}

		[Rule("<FunctionCall> ::= ~OBJECT_ID ~'(' StringLiteral ~',' StringLiteral ~')'")]
		public ObjectIdFunction(StringLiteral objectName, StringLiteral objectType): this(ScriptParser.ParseObjectName(objectName.Value), objectName.IsUnicode, objectType) {}

		public IQualified<SchemaName> ObjectName {
			get {
				return objectName;
			}
		}

		public StringLiteral ObjectType {
			get {
				return objectType;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteFunction("OBJECT_ID");
			writer.Write('(');
			using (StringWriter nameWriter = new StringWriter(CultureInfo.InvariantCulture)) {
				objectName.WriteTo(new SqlWriter(nameWriter, writer.Engine, SqlWriterMode.NoComments));
				new StringLiteral(nameWriter.ToString(), unicodeObjectName, null).WriteTo(writer);
			}
			if (objectType != null) {
				writer.Write(", ");
				objectType.WriteTo(writer);
			}
			writer.Write(')');
		}
	}
}
