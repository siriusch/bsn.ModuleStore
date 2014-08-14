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

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class TryCatchStatement: Statement {
		private readonly List<Statement> catchStatements;
		private readonly List<Statement> tryStatements;

		[Rule("<TryCatchStatement> ::= ~BEGIN ~TRY <StatementList> ~END ~TRY ~BEGIN ~CATCH <StatementList> ~END ~CATCH")]
		public TryCatchStatement(Sequence<Statement> tryStatements, Sequence<Statement> catchStatements) {
			this.tryStatements = tryStatements.ToList();
			this.catchStatements = catchStatements.ToList();
		}

		public IEnumerable<Statement> CatchStatements {
			get {
				return catchStatements;
			}
		}

		public IEnumerable<Statement> TryStatements {
			get {
				return tryStatements;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			writer.WriteKeyword("BEGIN TRY");
			using (writer.Indent()) {
				writer.WriteScriptSequence(tryStatements, WhitespacePadding.NewlineBefore, w => w.Write(';'));
				writer.WriteLine(";");
			}
			writer.WriteKeyword("END TRY");
			writer.WriteLine();
			writer.WriteKeyword("BEGIN CATCH");
			using (writer.Indent()) {
				writer.WriteScriptSequence(catchStatements, WhitespacePadding.NewlineBefore, w => w.Write(';'));
				writer.WriteLine(";");
			}
			writer.WriteKeyword("END CATCH");
		}
	}
}
