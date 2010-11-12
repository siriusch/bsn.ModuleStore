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
using bsn.ModuleStore.Mapper;
using bsn.ModuleStore.Sql.Script;

namespace bsn.ModuleStore.Bootstrapper {
	internal class DatabaseObject {
		[SqlColumn("iObject", Identity = true)]
		private int id;

		[SqlColumn("sSchema")]
		private string schemaName;

		[SqlColumn("sObject")]
		private string objectName;

		[SqlColumn("sType")]
		private string type;

		[SqlColumn("sDefinition")]
		private string definition;

		internal DatabaseObject(int id, string schemaName, string objectName, string type, string definition) {
			this.id = id;
			this.schemaName = schemaName;
			this.objectName = objectName;
			this.type = type;
			this.definition = definition;
		}

		public int Id {
			get {
				return id;
			}
		}

		public string SchemaName {
			get {
				return schemaName;
			}
		}

		public string ObjectName {
			get {
				return objectName;
			}
		}

		public ObjectCategory Category {
			get {
				switch (type) {
				case "U":
					return ObjectCategory.Table;
				case "V":
					return ObjectCategory.View;
				case "TR":
					return ObjectCategory.Trigger;
				case "P":
					//				case "PC":
					return ObjectCategory.Procedure;
				case "FN":
				case "IF":
				case "TF":
					//				case "FS":
					//				case "FT":
					//				case "AF":
					return ObjectCategory.Function;
				default:
					return ObjectCategory.None;
				}
			}
		}

		public string Type {
			get {
				return type;
			}
		}

		public string Definition {
			get {
				return definition;
			}
		}
	}
}