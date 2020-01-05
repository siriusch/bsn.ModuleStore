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

namespace bsn.ModuleStore.Sql.Script {
	public abstract class StatementFragment<TCreateStatement>: SqlScriptableToken, IAlterableCreateStatement where TCreateStatement: CreateStatement {
		private readonly TCreateStatement owner;

		protected StatementFragment(TCreateStatement owner) {
			this.owner = owner;
		}

		public TCreateStatement Owner => owner;

		public virtual string ObjectName => owner.ObjectName;

		public virtual bool DisableUsagesForUpdate => (owner is IAlterableCreateStatement alterable) && alterable.DisableUsagesForUpdate;

		public virtual ObjectCategory ObjectCategory => owner.ObjectCategory;

		public virtual IInstallStatement CreateAlterStatement() {
			if (AlterUsingUpdateScript) {
				throw new NotSupportedException($"{ObjectCategory} objects must be altered using an update script");
			}
			return new CompoundInstallStatement(ObjectName, CreateDropStatement(), this);
		}

		public abstract IInstallStatement CreateDropStatement();

		bool IAlterableCreateStatement.DoesApplyToEngine(DatabaseEngine engine) {
			return owner.DoesApplyToEngine(engine);
		}

		public virtual IEnumerable<T> GetReferencedObjectNames<T>() where T: SqlName {
			return owner.GetReferencedObjectNames<T>(null);
		}

		public override void WriteTo(SqlWriter writer) {
			owner.WriteTo(writer);
		}

		public virtual bool AlterUsingUpdateScript => false;

		public virtual bool IsPartOfSchemaDefinition => false;
	}
}
