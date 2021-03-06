﻿// bsn ModuleStore database versioning
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

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CreateXmlSchemaCollectionStatement: AlterableCreateStatement {
		private readonly Expression expression;
		private readonly Qualified<SchemaName, XmlSchemaCollectionName> xmlSchemaCollectionName;

		[Rule("<CreateXmlSchemaCollectionStatement> ::= ~CREATE ~XML ~SCHEMA ~COLLECTION <XmlSchemaCollectionNameQualified> ~AS <VariableName>")]
		public CreateXmlSchemaCollectionStatement(Qualified<SchemaName, XmlSchemaCollectionName> xmlSchemaCollectionName, VariableName variable): this(xmlSchemaCollectionName, ExpressionValue<VariableName>.CreateFrom(variable)) {}

		[Rule("<CreateXmlSchemaCollectionStatement> ::= ~CREATE ~XML ~SCHEMA ~COLLECTION <XmlSchemaCollectionNameQualified> ~AS StringLiteral")]
		public CreateXmlSchemaCollectionStatement(Qualified<SchemaName, XmlSchemaCollectionName> xmlSchemaCollectionName, Expression expression) {
			Debug.Assert(xmlSchemaCollectionName != null);
			Debug.Assert(expression != null);
			this.xmlSchemaCollectionName = xmlSchemaCollectionName;
			this.expression = expression;
		}

		public Expression Expression => expression;

		public override ObjectCategory ObjectCategory => ObjectCategory.XmlSchema;

		public override string ObjectName {
			get => xmlSchemaCollectionName.Name.Value;
			set => xmlSchemaCollectionName.Name = new XmlSchemaCollectionName(value);
		}

		public Qualified<SchemaName, XmlSchemaCollectionName> XmlSchemaCollectionName => xmlSchemaCollectionName;

		protected override SchemaName SchemaName => xmlSchemaCollectionName.Qualification;

		public override void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			writer.WriteKeyword("CREATE XML SCHEMA COLLECTION ");
			writer.WriteScript(xmlSchemaCollectionName, WhitespacePadding.None);
			writer.WriteKeyword(" AS ");
			writer.WriteScript(expression, WhitespacePadding.None);
		}

		protected override IInstallStatement CreateDropStatement() {
			return new DropXmlSchemaCollectionStatement(xmlSchemaCollectionName);
		}
	}
}
