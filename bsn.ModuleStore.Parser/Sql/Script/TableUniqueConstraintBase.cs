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

using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class TableUniqueConstraintBase: TableConstraint {
		private readonly Clustered clustered;
		private readonly ConstraintIndex constraintIndex;
		private readonly List<IndexColumn> indexColumns;

		protected TableUniqueConstraintBase(ConstraintName constraintName, ConstraintClusterToken clustered, Sequence<IndexColumn> indexColumns, ConstraintIndex constraintIndex): base(constraintName) {
			this.constraintIndex = constraintIndex;
			this.clustered = clustered.Clustered;
			this.indexColumns = indexColumns.ToList();
		}

		public Clustered Clustered {
			get {
				return clustered;
			}
		}

		public ConstraintIndex ConstraintIndex {
			get {
				return constraintIndex;
			}
		}

		public IEnumerable<IndexColumn> IndexColumns {
			get {
				return indexColumns;
			}
		}

		protected abstract string UniqueKindName {
			get;
		}

		public override void WriteTo(SqlWriter writer) {
			base.WriteTo(writer);
			writer.Write(UniqueKindName);
			writer.Write(' ');
			writer.WriteEnum(clustered, WhitespacePadding.SpaceAfter);
			writer.Write('(');
			writer.IncreaseIndent();
			writer.WriteScriptSequence(indexColumns, WhitespacePadding.NewlineBefore, ", ");
			writer.DecreaseIndent();
			writer.WriteLine();
			writer.Write(')');
			writer.WriteScript(constraintIndex, WhitespacePadding.None);
		}
	}
}
