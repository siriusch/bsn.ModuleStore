﻿// bsn ModuleStore database versioning
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

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("QuotedId")]
	public sealed class QuotedIdentifier: Identifier {
		internal static bool TryDequote(string value, out string result) {
			if ((value != null) && (value.Length > 2)) {
				switch (value[0]) {
				case '[':
					if (value.IndexOf(']') == value.Length-1) {
						result = value.Substring(1, value.Length-2);
						return true;
					}
					break;
				case '"':
					if (value[value.Length-1] == '"') {
						result = value.Substring(1, value.Length-2).Replace(@"""""", @"""");
						return true;
					}
					break;
				}
			}
			result = null;
			return false;
		}

		private readonly string value;

		public QuotedIdentifier(string id): base(id) {
			if (!TryDequote(id, out value)) {
				throw new ArgumentException("Malformed identifier", nameof(id));
			}
		}

		public override string Value => value;

		public override void WriteTo(SqlWriter writer) {
			writer.WriteDelimitedIdentifier(value);
		}
	}
}
