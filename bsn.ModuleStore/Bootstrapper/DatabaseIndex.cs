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
using System.Linq;
using System.Xml.Linq;

using bsn.ModuleStore.Mapper;

namespace bsn.ModuleStore.Bootstrapper {
	public class DatabaseIndex {
		[SqlColumn("xDefinition")]
		private readonly XDocument definition;

		[SqlColumn("iFillFactor")]
		private readonly int fillFactor;

		[SqlColumn("bIgnoreDupKey")]
		private readonly bool ignoreDuplicateKey;

		[SqlColumn("sIndex")]
		private readonly string name;

		[SqlColumn("iObject")]
		private readonly int objectId;

		[SqlColumn("bPrimaryKey")]
		private readonly bool primaryKey;

		[SqlColumn("iType")]
		private readonly DatabaseIndexType type;

		[SqlColumn("bUnique")]
		private readonly bool unique;

		[SqlColumn("bUniqueConstraint")]
		private readonly bool uniqueConstraint;

		internal DatabaseIndex(int objectId, string name, DatabaseIndexType type, bool unique, bool ignoreDuplicateKey, bool primaryKey, bool uniqueConstraint, int fillFactor, XDocument definition) {
			this.objectId = objectId;
			this.name = name;
			this.type = type;
			this.unique = unique;
			this.ignoreDuplicateKey = ignoreDuplicateKey;
			this.primaryKey = primaryKey;
			this.uniqueConstraint = uniqueConstraint;
			this.fillFactor = fillFactor;
			this.definition = definition;
		}

		public XDocument Definition {
			get {
				return definition;
			}
		}

		public int FillFactor {
			get {
				return fillFactor;
			}
		}

		public bool IgnoreDuplicateKey {
			get {
				return ignoreDuplicateKey;
			}
		}

		public string Name {
			get {
				return name;
			}
		}

		public int ObjectId {
			get {
				return objectId;
			}
		}

		public bool PrimaryKey {
			get {
				return primaryKey;
			}
		}

		public DatabaseIndexType Type {
			get {
				return type;
			}
		}

		public bool Unique {
			get {
				return unique;
			}
		}

		public bool UniqueConstraint {
			get {
				return uniqueConstraint;
			}
		}
	}
}