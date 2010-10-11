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
	public sealed class IfStatement: Statement {
		private readonly Predicate condition;
		private readonly Statement elseStatement;
		private readonly Statement thenStatement;

		[Rule("<IfStatement> ::= ~IF <Predicate> <StatementGroup>")]
		public IfStatement(Predicate condition, Statement thenStatement): this(condition, thenStatement, null) {}

		[Rule("<IfStatement> ::= ~IF <Predicate> <StatementBlock> ~ELSE <StatementGroup>")]
		public IfStatement(Predicate condition, Statement thenStatement, Statement elseStatement) {
			Debug.Assert(condition != null);
			Debug.Assert(thenStatement != null);
			this.condition = condition;
			this.thenStatement = thenStatement;
			this.elseStatement = elseStatement;
		}

		public Predicate Condition {
			get {
				return condition;
			}
		}

		public Statement ElseStatement {
			get {
				return elseStatement;
			}
		}

		public Statement ThenStatement {
			get {
				return thenStatement;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			writer.Write("IF ");
			writer.IncreaseIndent();
			writer.WriteScript(condition, WhitespacePadding.SpaceAfter);
			writer.DecreaseIndent();
			writer.WriteScript(thenStatement, WhitespacePadding.None);
			writer.WriteScript(elseStatement, WhitespacePadding.None, " ELSE ", null);
		}
	}
}
