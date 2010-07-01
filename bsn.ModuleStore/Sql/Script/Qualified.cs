using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class Qualified<T>: SqlToken {
		private readonly T name;
		private readonly Name qualification;

		[Rule("<ColumnNameQualified> ::= <ColumnName>", typeof(Name))]
		[Rule("<ColumnWildQualified> ::= <ColumnWild>", typeof(Name))]
		[Rule("<ProcedureNameQualified> ::=<ProcedureName>", typeof(Name))]
		[Rule("<TableNameQualified> ::= <TableName>", typeof(Name))]
		[Rule("<TypeNameQualified> ::= <TypeName>", typeof(TypeName))]
		public Qualified(T name): this(null, name) {}

		[Rule("<ColumnNameQualified> ::= <TableName> '.' <ColumnName>", typeof(Name))]
		[Rule("<ColumnNameQualified> ::= <VariableName> '.' <ColumnName>", typeof(Name))]
		[Rule("<ColumnWildQualified> ::= <TableName> '.' <ColumnWild>", typeof(Name))]
		[Rule("<ColumnWildQualified> ::= <VariableName> '.' <ColumnWild>", typeof(Name))]
		[Rule("<ProcedureNameQualified> ::= <SchemaName> '.' <ProcedureName>", typeof(Name))]
		[Rule("<TableNameQualified> ::= <SchemaName> '.' <TableName>", typeof(Name))]
		[Rule("<TypeNameQualified> ::= <SchemaName> '.' <TypeName>", typeof(TypeName))]
		public Qualified(Name qualification, InsignificantToken dot, T name): this(qualification, name) {}

		private Qualified(Name qualification, T name) {
			this.qualification = qualification;
			this.name = name;
		}

		public string FullName {
			get {
				if (qualification != null) {
					return string.Format("{0}.{1}", qualification, name);
				}
				return name.ToString();
			}
		}

		public bool IsQualified {
			get {
				return qualification != null;
			}
		}
		
		public T Name {
			get {
				return name;
			}
		}

		public Name Qualification {
			get {
				return qualification;
			}
		}
	}
}
