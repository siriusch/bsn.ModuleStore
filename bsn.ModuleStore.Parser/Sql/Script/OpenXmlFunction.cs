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
using System.Diagnostics;
using System.Globalization;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class OpenxmlFunction: SqlScriptableToken {
		private readonly int flags;
		private readonly OpenxmlSchema schema;
		private readonly SqlScriptableToken stringValue;
		private readonly VariableName variableName;

		[Rule("<Openxml> ::= ~OPENXML ~'(' <VariableName> ~',' <StringLiteral> ~')' <OptionalOpenxmlSchema>")]
		[Rule("<Openxml> ::= ~OPENXML ~'(' <VariableName> ~',' <VariableName> ~')' <OptionalOpenxmlSchema>")]
		public OpenxmlFunction(VariableName variableName, SqlScriptableToken stringValue, Optional<OpenxmlSchema> schema): this(variableName, stringValue, null, schema) {}

		[Rule("<Openxml> ::= ~OPENXML ~'(' <VariableName> ~',' <StringLiteral> ~',' <IntegerLiteral> ~')' <OptionalOpenxmlSchema>")]
		[Rule("<Openxml> ::= ~OPENXML ~'(' <VariableName> ~',' <VariableName> ~',' <IntegerLiteral> ~')' <OptionalOpenxmlSchema>")]
		public OpenxmlFunction(VariableName variableName, SqlScriptableToken stringValue, IntegerLiteral flags, Optional<OpenxmlSchema> schema) {
			Debug.Assert(variableName != null);
			Debug.Assert(stringValue != null);
			Debug.Assert(schema != null);
			this.variableName = variableName;
			this.stringValue = stringValue;
			this.flags = (int)flags.Value;
			this.schema = schema;
		}

		public int Flags {
			get {
				return flags;
			}
		}

		public OpenxmlSchema Schema {
			get {
				return schema;
			}
		}

		public SqlScriptableToken StringValue {
			get {
				return stringValue;
			}
		}

		public VariableName VariableName {
			get {
				return variableName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("OPENXML (");
			writer.WriteScript(variableName, WhitespacePadding.None);
			writer.Write(", ");
			stringValue.WriteTo(writer);
			if (flags != 0) {
				writer.Write(", ");
				writer.Write(flags.ToString(NumberFormatInfo.InvariantInfo));
			}
			writer.Write(')');
			writer.WriteScript(schema, WhitespacePadding.SpaceBefore);
		}
	}
}
