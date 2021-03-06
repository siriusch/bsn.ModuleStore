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

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("HexLiteral")]
	public sealed class IntegerHexLiteral: IntegerLiteral {
		private static long BinaryToLong(byte[] binary) {
			long result = 0;
			for (var i = 0; i < binary.Length; i++) {
				result = (result<<8)+binary[i];
			}
			return result;
		}

		private static int HexCharToInt(char ch) {
			switch (ch) {
			case '1':
				return 1;
			case '2':
				return 2;
			case '3':
				return 3;
			case '4':
				return 4;
			case '5':
				return 5;
			case '6':
				return 6;
			case '7':
				return 7;
			case '8':
				return 8;
			case '9':
				return 9;
			case 'A':
			case 'a':
				return 10;
			case 'B':
			case 'b':
				return 11;
			case 'C':
			case 'c':
				return 12;
			case 'D':
			case 'd':
				return 13;
			case 'E':
			case 'e':
				return 14;
			case 'F':
			case 'f':
				return 15;
			}
			return 0;
		}

		private static char IntToHexChar(int i) {
			unchecked {
				return (char)('0'+(i / 10) * 7+i);
			}
		}

		internal static byte[] ParseBinary(string value) {
			var result = new byte[(value.Length-1) / 2];
			var offset = 2-(value.Length % 2);
			unchecked {
				for (var i = 0; i < result.Length; i++) {
					result[i] = (byte)((HexCharToInt(value[offset++])<<4)+HexCharToInt(value[offset++]));
				}
			}
			return result;
		}

		private readonly byte[] binaryValue;

		public IntegerHexLiteral(string value): this(ParseBinary(value)) { }

		internal IntegerHexLiteral(byte[] binaryValue): base(BinaryToLong(binaryValue)) {
			this.binaryValue = binaryValue;
		}

		public byte[] BinaryValue => binaryValue;

		public bool IsLongFitting() {
			for (var i = 8; i < binaryValue.Length; i++) {
				if (binaryValue[i] > 0) {
					return false;
				}
			}
			return true;
		}

		public override bool TryGetNegativeAsPositive(out Literal literal) {
			literal = null;
			return false;
		}

		public override void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			writer.WriteLiteral("0x");
			for (var i = 0; i < binaryValue.Length; i++) {
				var b = binaryValue[i];
				writer.WriteLiteral(IntToHexChar(b>>4));
				writer.WriteLiteral(IntToHexChar(b&15));
			}
		}
	}
}
