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
//  
using System;
using System.Diagnostics;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class CreateTypeStatement: CreateStatement {
		private readonly Qualified<SchemaName, TypeName> typeName;

		protected CreateTypeStatement(Qualified<SchemaName, TypeName> typeName) {
			Debug.Assert(typeName != null);
			this.typeName = typeName;
		}

		public override ObjectCategory ObjectCategory {
			get {
				return ObjectCategory.Type;
			}
		}

		public override string ObjectName {
			get {
				return typeName.Name.Value;
			}
			set {
				typeName.Name = new TypeName(value);
			}
		}

		public Qualified<SchemaName, TypeName> TypeName {
			get {
				return typeName;
			}
		}

		// ReSharper disable RedundantOverridenMember
		public override Statement CreateAlterStatement() {
			// TODO: implement the full ALTER replacement sequence
			// See http://stackoverflow.com/questions/1383494/alter-user-defined-type-in-sql-server#answer-1383509
			return base.CreateAlterStatement();
		}
		// ReSharper restore RedundantOverridenMember

		public override DropStatement CreateDropStatement() {
			return new DropTypeStatement(typeName);
		}

		protected override string GetObjectSchema() {
			return typeName.IsQualified ? typeName.Qualification.Value : string.Empty;
		}

		public override void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			writer.Write("CREATE TYPE ");
			writer.WriteScript(typeName, WhitespacePadding.SpaceAfter);
		}
	}
}