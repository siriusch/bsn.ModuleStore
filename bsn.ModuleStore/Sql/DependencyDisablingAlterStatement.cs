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
using System.Linq;

using bsn.ModuleStore.Sql.Script;

namespace bsn.ModuleStore.Sql {
	internal class DependencyDisablingAlterStatement: IInstallStatement {
		private readonly IInstallStatement alterStatement;

		public DependencyDisablingAlterStatement(IInstallStatement alterStatement) {
			this.alterStatement = alterStatement;
		}

		public ICollection<IAlterableCreateStatement> GetDependencyObjects(Inventory inventory, ICollection<string> droppedObjects) {
			return
					inventory.Objects
					         .OfType<IAlterableCreateStatement>()
					         .Where(o => (!StringComparer.OrdinalIgnoreCase.Equals(o.ObjectName, alterStatement.ObjectName)) && (!droppedObjects.Contains(o.ObjectName)) && o.GetReferencedObjectNames<SqlName>().Any(n => StringComparer.OrdinalIgnoreCase.Equals(n.Value, alterStatement.ObjectName)))
					         .ToList();
		}

		public string ObjectName {
			get {
				return alterStatement.ObjectName;
			}
		}

		public bool Equals(IHashableStatement other, DatabaseEngine engine) {
			return alterStatement.Equals(other, engine);
		}

		public byte[] GetHash(DatabaseEngine engine) {
			return alterStatement.GetHash(engine);
		}

		public IEnumerable<T> GetReferencedObjectNames<T>() where T: SqlName {
			return alterStatement.GetReferencedObjectNames<T>();
		}

		public void WriteTo(SqlWriter writer) {
			alterStatement.WriteTo(writer);
		}

		public bool IsPartOfSchemaDefinition {
			get {
				return alterStatement.IsPartOfSchemaDefinition;
			}
		}
	}
}
