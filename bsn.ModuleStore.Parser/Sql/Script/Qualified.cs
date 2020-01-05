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

using bsn.GoldParser.Parser;
using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class Qualified<TQ, TN>: SqlScriptableToken, IQualifiedName<TQ> where TQ: SqlName
			where TN: SqlName {
		private readonly TQ qualification;
		private bool lockedOverride;
		private TN name;
		private IQualified<TQ> qualificationOverride;

		[Rule("<ColumnNameQualified> ::= <ColumnName>", typeof(SqlName), typeof(ColumnName))]
		[Rule("<ColumnWildQualified> ::= <ColumnWild>", typeof(SqlName), typeof(ColumnName))]
		[Rule("<ProcedureNameQualified> ::= <ProcedureName>", typeof(SchemaName), typeof(ProcedureName))]
		[Rule("<FunctionNameQualified> ::= <FunctionName>", typeof(SchemaName), typeof(FunctionName))]
		[Rule("<TableNameQualified> ::= <TableName>", typeof(SchemaName), typeof(TableName))]
		[Rule("<SimpleTypeNameQualified> ::= <SimpleTypeName>", typeof(SchemaName), typeof(TypeName))]
		[Rule("<TypeNameQualified> ::= <TypeName>", typeof(SchemaName), typeof(TypeName))]
		[Rule("<ViewNameQualified> ::= <ViewName>", typeof(SchemaName), typeof(ViewName))]
		[Rule("<XmlSchemaCollectionNameQualified> ::= <XmlSchemaCollectionName>", typeof(SchemaName), typeof(XmlSchemaCollectionName))]
		[Rule("<TriggerNameQualified> ::= <TriggerName>", typeof(SchemaName), typeof(TriggerName))]
		[Rule("<ObjectNameQualified> ::= <ObjectName>", typeof(SchemaName), typeof(ObjectName))]
		public Qualified(TN name): this(null, name) {}

		[Rule("<ColumnNameQualified> ::= <TableName> ~'.' <ColumnName>", typeof(SqlName), typeof(ColumnName))]
		[Rule("<ColumnWildQualified> ::= <TableName> ~'.' <ColumnWild>", typeof(SqlName), typeof(ColumnName))]
		[Rule("<ColumnWildQualified> ::= <VariableName> ~'.' <ColumnWild>", typeof(SqlName), typeof(ColumnName))]
		[Rule("<ProcedureNameQualified> ::= <SchemaName> ~'.' <ProcedureName>", typeof(SchemaName), typeof(ProcedureName))]
		[Rule("<FunctionNameQualified> ::= <SchemaName> ~'.' <FunctionName>", typeof(SchemaName), typeof(FunctionName))]
		[Rule("<TableNameQualified> ::= <SchemaName> ~'.' <TableName>", typeof(SchemaName), typeof(TableName))]
		[Rule("<SimpleTypeNameQualified> ::= <SchemaName> ~'.' <SimpleTypeName>", typeof(SchemaName), typeof(TypeName))]
		[Rule("<TypeNameQualified> ::= <SchemaName> ~'.' <TypeName>", typeof(SchemaName), typeof(TypeName))]
		[Rule("<ViewNameQualified> ::= <SchemaName> ~'.' <ViewName>", typeof(SchemaName), typeof(ViewName))]
		[Rule("<XmlSchemaCollectionNameQualified> ::= <SchemaName> ~'.' <XmlSchemaCollectionName>", typeof(SchemaName), typeof(XmlSchemaCollectionName))]
		[Rule("<TriggerNameQualified> ::= <SchemaName> ~'.' <TriggerName>", typeof(SchemaName), typeof(TriggerName))]
		[Rule("<ObjectNameQualified> ::= <SchemaName> ~'.' <ObjectName>", typeof(SchemaName), typeof(ObjectName))]
		public Qualified(TQ qualification, TN name) {
			Debug.Assert(name != null);
			this.qualification = qualification;
			this.name = name;
		}

		public string FullName => ToString();

		public bool IsQualified => Qualification != null;

		public TN Name {
			get => name;
			internal set {
				if (value == null) {
					throw new ArgumentNullException(nameof(value));
				}
				name = value;
			}
		}

		public override int GetHashCode() {
			var hashCode = name.GetHashCode();
			if ((qualificationOverride == null) && (qualification != null)) {
				hashCode ^= qualification.GetHashCode();
			}
			return hashCode;
		}

		public override void WriteTo(SqlWriter writer) {
			var currentQualification = Qualification;
			var isQualified = currentQualification != null;
			if (isQualified) {
				currentQualification.WriteToInternal(writer, true);
				writer.Write('.');
			}
			name.WriteToInternal(writer, isQualified);
		}

		internal void SetPosition(LineInfo position) {
			Initialize(((IToken)name).Symbol, position);
		}

		public bool IsOverridden => qualificationOverride != null;

		public bool LockedOverride => lockedOverride;

		public int CompareTo(IQualifiedName<TQ> other) {
			if (other != null) {
				if (other == this) {
					return 0;
				}
				var currentQualification = Qualification;
				if (currentQualification == null) {
					if (other.Qualification != null) {
						return -1;
					}
				} else {
					var diff = currentQualification.CompareTo(other.Qualification);
					if (diff != 0) {
						return diff;
					}
				}
				return name.CompareTo(other.Name);
			}
			return 1;
		}

		public bool Equals(IQualifiedName<TQ> other) {
			if (other != null) {
				if (other == this) {
					return true;
				}
				var currentQualification = Qualification;
				if (currentQualification == null) {
					if (other.Qualification != null) {
						return false;
					}
				} else {
					if (!currentQualification.Equals(other.Qualification)) {
						return false;
					}
				}
				return name.Equals(other.Name);
			}
			return false;
		}

		SqlName IQualifiedName<TQ>.Name => Name;

		void IQualifiedName<TQ>.SetOverride(IQualified<TQ> qualificationProvider) {
			if (!lockedOverride) {
				if (qualificationProvider == this) {
					throw new ArgumentException("Cannot assign itself as override", nameof(qualificationProvider));
				}
				qualificationOverride = qualificationProvider;
			}
		}

		public void LockOverride() {
			lockedOverride = true;
		}

		public TQ Qualification {
			get {
				if (qualificationOverride != null) {
					return qualificationOverride.Qualification;
				}
				return qualification;
			}
		}
	}
}
