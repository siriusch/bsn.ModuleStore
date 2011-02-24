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

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CreateTypeFromStatement: CreateTypeStatement {
		private readonly TypeName systemTypeName;
		private readonly TypeConstraintToken constraint;

		[Rule("<CreateTypeStatement> ::= ~CREATE ~TYPE <SimpleTypeNameQualified> ~FROM <TypeName> <TypeConstraint>")]
		public CreateTypeFromStatement(Qualified<SchemaName, TypeName> typeName, TypeName systemTypeName, TypeConstraintToken constraint): base(typeName) {
			if (!systemTypeName.IsBuiltinType) {
				throw new ArgumentException("Derived types can only be created from system types", "systemTypeName");
			}
			this.systemTypeName = systemTypeName;
			this.constraint = constraint;
		}

		public TypeName SystemTypeName {
			get {
				return systemTypeName;
			}
		}

		public TypeConstraint Constraint {
			get {
				return constraint.Constraint;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			base.WriteTo(writer);
			writer.Write("FROM");
			writer.WriteScript(systemTypeName, WhitespacePadding.SpaceBefore);
			writer.WriteScript(constraint, WhitespacePadding.SpaceBefore);
		}
	}
}