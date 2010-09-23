using System;
using System.Diagnostics;

using bsn.GoldParser.Parser;
using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class Qualified<TQ, TN>: SqlScriptableToken, IQualifiedName<TQ> where TQ: SqlName
	                                                                              where TN: SqlName {
		private readonly TN name;
		private readonly TQ qualification;
		private IQualified<TQ> qualificationOverride;
		private bool lockedOverride;

		[Rule("<ColumnNameQualified> ::= <ColumnName>", typeof(SqlName), typeof(ColumnName))]
		[Rule("<ColumnWildQualified> ::= <ColumnWild>", typeof(SqlName), typeof(ColumnName))]
		[Rule("<ProcedureNameQualified> ::= <ProcedureName>", typeof(SchemaName), typeof(ProcedureName))]
		[Rule("<FunctionNameQualified> ::= <FunctionName>", typeof(SchemaName), typeof(FunctionName))]
		[Rule("<TableNameQualified> ::= <TableName>", typeof(SchemaName), typeof(TableName))]
		[Rule("<TypeNameQualified> ::= <TypeName>", typeof(SchemaName), typeof(TypeName))]
		[Rule("<ViewNameQualified> ::= <ViewName>", typeof(SchemaName), typeof(ViewName))]
		[Rule("<XmlSchemaCollectionNameQualified> ::= <XmlSchemaCollectionName>", typeof(SchemaName), typeof(XmlSchemaCollectionName))]
		[Rule("<TriggerNameQualified> ::= <TriggerName>", typeof(SchemaName), typeof(TriggerName))]
		public Qualified(TN name): this(null, name) {}

		[Rule("<ColumnNameQualified> ::= <TableName> ~'.' <ColumnName>", typeof(SqlName), typeof(ColumnName))]
		[Rule("<ColumnNameQualified> ::= <VariableName> ~'.' <ColumnName>", typeof(SqlName), typeof(ColumnName))]
		[Rule("<ColumnWildQualified> ::= <TableName> ~'.' <ColumnWild>", typeof(SqlName), typeof(ColumnName))]
		[Rule("<ColumnWildQualified> ::= <VariableName> ~'.' <ColumnWild>", typeof(SqlName), typeof(ColumnName))]
		[Rule("<ProcedureNameQualified> ::= <SchemaName> ~'.' <ProcedureName>", typeof(SchemaName), typeof(ProcedureName))]
		[Rule("<FunctionNameQualified> ::= <SchemaName> ~'.' <FunctionName>", typeof(SchemaName), typeof(FunctionName))]
		[Rule("<TableNameQualified> ::= <SchemaName> ~'.' <TableName>", typeof(SchemaName), typeof(TableName))]
		[Rule("<TypeNameQualified> ::= <SchemaName> ~'.' <TypeName>", typeof(SchemaName), typeof(TypeName))]
		[Rule("<ViewNameQualified> ::= <SchemaName> ~'.' <ViewName>", typeof(SchemaName), typeof(ViewName))]
		[Rule("<XmlSchemaCollectionNameQualified> ::= <SchemaName> ~'.' <XmlSchemaCollectionName>", typeof(SchemaName), typeof(XmlSchemaCollectionName))]
		[Rule("<TriggerNameQualified> ::= <SchemaName> ~'.' <TriggerName>", typeof(SchemaName), typeof(TriggerName))]
		public Qualified(TQ qualification, TN name) {
			Debug.Assert(name != null);
			this.qualification = qualification;
			this.name = name;
		}

		public string FullName {
			get {
				return ToString();
			}
		}

		public bool IsQualified {
			get {
				return Qualification != null;
			}
		}

		public TN Name {
			get {
				return name;
			}
		}

		public bool IsOverridden {
			get {
				return qualificationOverride != null;
			}
		}

		public bool LockedOverride {
			get {
				return lockedOverride;
			}
		}

		public override int GetHashCode() {
			int hashCode = name.GetHashCode();
			if ((qualificationOverride == null) && (qualification != null)) {
				hashCode ^= qualification.GetHashCode();
			}
			return hashCode;
		}

		public override void WriteTo(SqlWriter writer) {
			TQ currentQualification = Qualification;
			bool isQualified = currentQualification != null;
			if (isQualified) {
				currentQualification.WriteToInternal(writer, true);
				writer.Write('.');
			}
			name.WriteToInternal(writer, isQualified);
		}

		internal void SetPosition(LineInfo position) {
			Initialize(((IToken)name).Symbol, position);
		}

		public int CompareTo(IQualifiedName<TQ> other) {
			if (other != null) {
				if (ReferenceEquals(this, other)) {
					return 0;
				}
				TQ currentQualification = Qualification;
				if (currentQualification == null) {
					if (other.Qualification != null) {
						return -1;
					}
				} else {
					int diff = currentQualification.CompareTo(other.Qualification);
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
				if (ReferenceEquals(this, other)) {
					return true;
				}
				TQ currentQualification = Qualification;
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

		SqlName IQualifiedName<TQ>.Name {
			get {
				return Name;
			}
		}

		void IQualifiedName<TQ>.SetOverride(IQualified<TQ> qualificationProvider) {
			if (!lockedOverride) {
				if (ReferenceEquals(this, qualificationProvider)) {
					throw new ArgumentException("Cannot assign itself as override", "qualificationProvider");
				}
				qualificationOverride = qualificationProvider;
			}
		}

		void IQualifiedName<TQ>.LockOverride() {
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
