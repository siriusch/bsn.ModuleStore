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
using System.Text;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("StringLiteral")]
	public sealed class StringLiteral: Literal<string> {
		internal static bool ParseIsUnicode(string value) {
			return value.StartsWith("N", StringComparison.OrdinalIgnoreCase);
		}

		internal static string ParseValue(string value) {
			StringBuilder result = new StringBuilder(value.Length-2);
			int i = 0;
			if (value[i] != '\'') {
				i++;
			}
			Debug.Assert(value[i] == '\'');
			i++;
			bool keepQuote = false;
			while (i < value.Length) {
				char c = value[i++];
				bool isNotQuote = c != '\'';
				if (isNotQuote || keepQuote) {
					result.Append(c);
					keepQuote = false;
				} else {
					keepQuote = true;
				}
			}
			return result.ToString();
		}

		private readonly CollationName collation;
		private readonly bool isUnicode;

		[Rule("<CollableStringLiteral> ::= StringLiteral ~COLLATE <CollationName>")]
		public StringLiteral(StringLiteral value, CollationName collation): this(value.Value, value.IsUnicode, collation) {}

		public StringLiteral(string value): this(ParseValue(value), ParseIsUnicode(value), null) {}

		private StringLiteral(string parsedValue, bool isUnicode, CollationName collation): base(parsedValue) {
			this.isUnicode = isUnicode;
			this.collation = collation;
		}

		public CollationName Collation {
			get {
				return collation;
			}
		}

		public bool IsUnicode {
			get {
				return isUnicode;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			writer.Write('\'');
			writer.Write(Value.Replace("'", "''"));
			writer.Write('\'');
			writer.WriteScript(collation, WhitespacePadding.SpaceBefore);
		}
	}
}
