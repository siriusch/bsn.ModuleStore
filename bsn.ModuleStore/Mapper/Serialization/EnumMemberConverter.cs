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
using System.Diagnostics;

namespace bsn.ModuleStore.Mapper.Serialization {
	internal class EnumMemberConverter: MemberConverter {
		private readonly Type underlyingType;

		public EnumMemberConverter(Type type, bool isIdentity, string columnName, int memberIndex): base(type, isIdentity, columnName, memberIndex) {
			underlyingType = Enum.GetUnderlyingType(type);
			Debug.Assert(underlyingType != null);
		}

		public override object ProcessFromDb(IDeserializerContext context, int column) {
			object result = base.ProcessFromDb(context, column);
			if (result != null) {
				switch (Type.GetTypeCode(result.GetType())) {
				case TypeCode.String:
					return Enum.Parse(Type, (string)result);
				case TypeCode.Byte:
					return Enum.ToObject(Type, (byte)result);
				case TypeCode.SByte:
					return Enum.ToObject(Type, (sbyte)result);
				case TypeCode.UInt16:
					return Enum.ToObject(Type, (ushort)result);
				case TypeCode.Int16:
					return Enum.ToObject(Type, (short)result);
				case TypeCode.UInt32:
					return Enum.ToObject(Type, (uint)result);
				case TypeCode.Int32:
					return Enum.ToObject(Type, (int)result);
				case TypeCode.UInt64:
					return Enum.ToObject(Type, (ulong)result);
				case TypeCode.Int64:
					return Enum.ToObject(Type, (long)result);
				default:
					result = Convert.ChangeType(result, underlyingType);
					Debug.Assert(result != null);
					return Enum.ToObject(Type, result);
				}
			}
			return result;
		}
	}
}
