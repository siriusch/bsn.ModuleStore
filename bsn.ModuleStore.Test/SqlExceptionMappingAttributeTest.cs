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
using System.Linq;
using System.Reflection;

using bsn.ModuleStore.Bootstrapper;
using bsn.ModuleStore.Mapper.AssemblyMetadata;

using Xunit;

namespace bsn.ModuleStore {
	public class SqlExceptionMappingAttributeTest {
		public enum Level {
			Assembly,
			Type,
			Method
		}

		private static readonly MemberInfo method = typeof(IModules).GetMethods().First(m => m.IsDefined(typeof(SqlProcedureAttribute), true));

		private static SqlExceptionMappingAttribute Create<T>(Level level, int? number, byte? severity, byte? state) where T: Exception {
			SqlExceptionMappingAttribute result = new SqlExceptionMappingAttribute(typeof(T));
			if (number.HasValue) {
				result.Number = number.Value;
			}
			if (severity.HasValue) {
				result.Severity = severity.Value;
			}
			if (state.HasValue) {
				result.State = state.Value;
			}
			MemberInfo appliedOn;
			switch (level) {
			case Level.Method:
				appliedOn = method;
				break;
			case Level.Type:
				appliedOn = method.DeclaringType;
				break;
			case Level.Assembly:
				appliedOn = null;
				break;
			default:
				throw new ArgumentOutOfRangeException("level");
			}
			((IHasDeclaringMember)result).SetDeclaringMember(appliedOn);
			return result;
		}

		[Theory]
		[InlineData(Level.Assembly, null, null, null, Level.Type, null, null, null)]
		[InlineData(Level.Assembly, null, null, null, Level.Method, null, null, null)]
		[InlineData(Level.Type, null, null, null, Level.Method, null, null, null)]
		[InlineData(Level.Assembly, null, null, null, Level.Assembly, 50000, null, null)]
		[InlineData(Level.Assembly, null, null, null, Level.Assembly, null, (byte)16, null)]
		[InlineData(Level.Assembly, null, null, null, Level.Assembly, null, null, (byte)0)]
		[InlineData(Level.Assembly, null, null, (byte)0, Level.Assembly, null, (byte)16, null)]
		[InlineData(Level.Assembly, null, null, (byte)0, Level.Assembly, 50000, null, null)]
		[InlineData(Level.Assembly, null, (byte)16, null, Level.Assembly, 50000, null, null)]
		[InlineData(Level.Assembly, null, (byte)10, null, Level.Assembly, null, (byte)16, null)]
		[InlineData(Level.Assembly, 51000, (byte)16, (byte)255, Level.Assembly, 50000, (byte)25, (byte)255)]
		[InlineData(Level.Assembly, 51000, (byte)16, (byte)255, Level.Assembly, 50000, (byte)17, (byte)0)]
		public void SpecificityCompare(Level xLevel, int? xMessageId, byte? xSeverity, byte? xState, Level yLevel, int? yMessageId, byte? ySeverity, byte? yState) {
			SqlExceptionMappingAttribute x = Create<Exception>(xLevel, xMessageId, xSeverity, xState);
			SqlExceptionMappingAttribute y = Create<Exception>(yLevel, yMessageId, ySeverity, yState);
			Assert.True(x.ComputeSpecificity() < y.ComputeSpecificity());
			Assert.True(((IComparable<SqlExceptionMappingAttribute>)x).CompareTo(y) < 0);
			Assert.True(((IComparable<SqlExceptionMappingAttribute>)y).CompareTo(x) > 0);
		}
	}
}
