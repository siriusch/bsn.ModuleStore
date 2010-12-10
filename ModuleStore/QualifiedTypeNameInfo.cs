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

namespace bsn.ModuleStore.Console {
	[Serializable]
	internal struct QualifiedTypeNameInfo {
		private readonly string assemblyName;
		private readonly string typeName;

		public QualifiedTypeNameInfo(Type type): this(type.Assembly.FullName, type.FullName) {}

		public QualifiedTypeNameInfo(string assemblyName, string typeName) {
			this.assemblyName = assemblyName;
			this.typeName = typeName;
		}

		public string AssemblyName {
			get {
				return assemblyName;
			}
		}

		public string TypeName {
			get {
				return typeName;
			}
		}

		public bool Equals(QualifiedTypeNameInfo other) {
			return Equals(other.assemblyName, assemblyName) && Equals(other.typeName, typeName);
		}

		public override bool Equals(object obj) {
			if (obj == null) {
				return false;
			}
			if (obj.GetType() != typeof(QualifiedTypeNameInfo)) {
				return false;
			}
			return Equals((QualifiedTypeNameInfo)obj);
		}

		public Type FindType(bool throwOnError) {
			return Type.GetType(GetAssemblyQualifiedName(), throwOnError, false);
		}

		public string GetAssemblyQualifiedName() {
			return String.Format("{0}, {1}", typeName, assemblyName);
		}

		public override int GetHashCode() {
			unchecked {
				return ((assemblyName != null ? assemblyName.GetHashCode() : 0)*397)^(typeName != null ? typeName.GetHashCode() : 0);
			}
		}

		public override string ToString() {
			return GetAssemblyQualifiedName();
		}
	}
}
