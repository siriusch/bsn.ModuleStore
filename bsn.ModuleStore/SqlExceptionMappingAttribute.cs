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
using System.Reflection;

namespace bsn.ModuleStore {
	[AttributeUsage(AttributeTargets.Assembly|AttributeTargets.Interface|AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
	public sealed class SqlExceptionMappingAttribute: SqlAssemblyAttribute, IHasDeclaringMember, IComparable<SqlExceptionMappingAttribute> {
		private readonly Type targetException;
		private MemberInfo declaredOn;
		private string message;
		private int? number;
		private bool @readonly;
		private byte? severity;
		private byte? state;

		public SqlExceptionMappingAttribute(Type targetException) {
			this.targetException = targetException;
		}

		public string Message {
			get {
				return message;
			}
			set {
				AssertNotReadonly();
				message = value;
			}
		}

		public int? Number {
			get {
				return number;
			}
			set {
				AssertNotReadonly();
				number = value;
			}
		}

		public byte? Severity {
			get {
				return severity;
			}
			set {
				AssertNotReadonly();
				severity = value;
			}
		}

		public byte? State {
			get {
				return state;
			}
			set {
				AssertNotReadonly();
				state = value;
			}
		}

		public Type TargetException {
			get {
				return targetException;
			}
		}

		internal int ComputeSpecificity() {
			int result = 0;
			if (declaredOn is Type) {
				result += 0x8000;
			} else if (declaredOn is MethodInfo) {
				result += 0x10000;
			}
			if (number.HasValue) {
				result += 0x4000;
			}
			if (severity.HasValue) {
				result += Math.Min(26, severity.Value+1)*0x200; // max 26*0x200 = 0x3400
			}
			if (state.HasValue) {
				result += state.Value+1;
			}
			return result;
		}

		private void AssertNotReadonly() {
			if (@readonly) {
				throw new InvalidOperationException("THe attribute is readonly");
			}
		}

		int IComparable<SqlExceptionMappingAttribute>.CompareTo(SqlExceptionMappingAttribute other) {
			return ComputeSpecificity()-other.ComputeSpecificity();
		}

		public MemberInfo DeclaredOn {
			get {
				return declaredOn;
			}
		}

		void IHasDeclaringMember.SetDeclaringMember(MemberInfo member) {
			AssertNotReadonly();
			declaredOn = member;
			@readonly = true;
		}
	}
}
