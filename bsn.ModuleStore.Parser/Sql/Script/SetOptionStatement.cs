// bsn ModuleStore database versioning
// -----------------------------------
// 
// Copyright 2010 by Ars�ne von Wyss - avw@gmx.ch
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
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class SetOptionStatement: Statement {
		private readonly string option;

		[Rule("<SetOptionStatement> ::= ~SET ROWCOUNT <SetValue>")]
		public SetOptionStatement(SqlScriptableToken identifier, SqlScriptableToken value): this(identifier, new Sequence<SqlScriptableToken>(value)) {}

		[Rule("<SetOptionStatement> ::= ~SET Id <SetValueList>")]
		[Rule("<SetOptionStatement> ::= ~SET TRANSACTION <SetValueList>")]
		[Rule("<SetOptionStatement> ::= ~SET OFFSETS <SetValueList>")]
		[Rule("<SetOptionStatement> ::= ~SET STATISTICS <SetValueList>")]
		public SetOptionStatement(SqlScriptableToken identifier, Sequence<SqlScriptableToken> valueList) {
			using (var stringWriter = new StringWriter()) {
				var writer = new SqlWriter(stringWriter, DatabaseEngine.Unknown);
				writer.WriteScript(identifier, WhitespacePadding.None);
				foreach (var token in valueList) {
					writer.Write(' ');
					token.WriteTo(writer);
				}
				option = stringWriter.ToString();
			}
		}

		public string Option => option;

		public override void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			writer.WriteKeyword("SET ");
			writer.WriteKeyword(option);
		}
	}
}
