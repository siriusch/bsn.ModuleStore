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
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class DeclareVariableItem: DeclareItem {
		private readonly Expression initialization;
		private readonly Qualified<SchemaName, TypeName> typeName;

		[Rule("<DeclareItem> ::= <VariableName> ~<OptionalAs> <TypeNameQualified>")]
		public DeclareVariableItem(VariableName variable, Qualified<SchemaName, TypeName> typeName): this(variable, typeName, null) {}

		[Rule("<DeclareItem> ::= <VariableName> ~<OptionalAs> <TypeNameQualified> ~'=' <Expression>")]
		public DeclareVariableItem(VariableName variable, Qualified<SchemaName, TypeName> typeName, Expression initialization): base(variable) {
			Debug.Assert(typeName != null);
			this.typeName = typeName;
			this.initialization = initialization;
		}

		public Expression Initialization {
			get {
				return initialization;
			}
		}

		public Qualified<SchemaName, TypeName> TypeName {
			get {
				return typeName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			base.WriteTo(writer);
			writer.WriteScript(typeName, WhitespacePadding.None);
			writer.WriteScript(initialization, WhitespacePadding.None, "=", null);
		}
	}
}
