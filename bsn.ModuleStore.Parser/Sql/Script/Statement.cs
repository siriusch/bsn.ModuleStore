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

namespace bsn.ModuleStore.Sql.Script {
	public abstract class Statement: CommentContainerToken, IScriptableStatement {
		internal IEnumerable<IQualifiedName<SchemaName>> GetObjectSchemaQualifiedNames(string schemaName) {
			return GetInnerSchemaQualifiedNames(n => n.Equals(schemaName, StringComparison.OrdinalIgnoreCase)).Where(qn => !qn.LockedOverride);
		}

		public IEnumerable<T> GetReferencedObjectNames<T>() where T: SqlName {
			return GetInnerTokens<IQualifiedName<SchemaName>>((qn, scope) => (!qn.LockedOverride) && qn.IsOverridden, null).Select(qn => qn.Name).OfType<T>().Distinct();
		}
	}
}
