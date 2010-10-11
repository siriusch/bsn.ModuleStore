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
	public sealed class WhileStatement: Statement {
		private readonly Predicate predicate;
		private readonly Statement statement;

		[Rule("<WhileStatement> ::= ~WHILE <Predicate> <StatementGroup>")]
		public WhileStatement(Predicate predicate, Statement statement) {
			Debug.Assert(predicate != null);
			Debug.Assert(statement != null);
			this.predicate = predicate;
			this.statement = statement;
		}

		public Predicate Predicate {
			get {
				return predicate;
			}
		}

		public Statement Statement {
			get {
				return statement;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			writer.Write("WHILE ");
			writer.WriteScript(predicate, WhitespacePadding.SpaceAfter);
			writer.WriteScript(statement, WhitespacePadding.None);
		}
	}
}
