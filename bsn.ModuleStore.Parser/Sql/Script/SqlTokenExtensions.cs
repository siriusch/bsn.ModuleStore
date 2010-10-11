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

using bsn.GoldParser.Grammar;
using bsn.GoldParser.Parser;

namespace bsn.ModuleStore.Sql.Script {
	public static class SqlTokenExtensions {
		internal static Identifier CreateIdentifier(this IToken token, Symbol identifierSymbol, string text) {
			return new Identifier(text, identifierSymbol, token.Position);
		}

		public static T[] ToArray<T>(this Optional<Sequence<T>> sequence) where T: SqlToken {
			if (!sequence.HasValue()) {
				return new T[0];
			}
			return ToArray(sequence.Value);
		}

		public static T[] ToArray<T>(this Sequence<T> sequence) where T: SqlToken {
			return ToList(sequence).ToArray();
		}

		public static List<T> ToList<T>(this Optional<Sequence<T>> sequence) where T: SqlToken {
			if (!sequence.HasValue()) {
				return new List<T>(0);
			}
			return ToList(sequence.Value);
		}

		public static List<T> ToList<T>(this Sequence<T> sequence) where T: SqlToken {
			List<T> result = new List<T>();
			if (sequence != null) {
				result.AddRange(sequence);
			}
			return result;
		}

		public static bool HasValue<T>(this Optional<T> optional) where T: SqlToken {
			return (optional != null) && (optional.Value != null);
		}
	}
}
