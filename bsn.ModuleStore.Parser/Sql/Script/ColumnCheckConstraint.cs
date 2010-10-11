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
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ColumnCheckConstraint: ColumnNamedConstraintBase {
		private readonly Expression expression;
		private readonly ReplicationToken replication;

		[Rule("<NamedColumnConstraint> ::= ~CHECK <OptionalNotForReplication> ~'(' <Expression> ~')'")]
		public ColumnCheckConstraint(ReplicationToken replication, Expression expression): this(null, replication, expression) {}

		[Rule("<NamedColumnConstraint> ::= ~CONSTRAINT <ConstraintName> ~CHECK <OptionalNotForReplication> ~'(' <Expression> ~')'")]
		public ColumnCheckConstraint(ConstraintName constraintName, ReplicationToken replication, Expression expression): base(constraintName) {
			Debug.Assert(expression != null);
			this.replication = replication;
			this.expression = expression;
		}

		public Expression Expression {
			get {
				return expression;
			}
		}

		public ReplicationToken Replication {
			get {
				return replication;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			base.WriteTo(writer);
			writer.Write("CHECK ");
			writer.WriteScript(replication, WhitespacePadding.SpaceAfter);
			writer.WriteScript(expression, WhitespacePadding.None);
			writer.Write(')');
		}
	}
}
