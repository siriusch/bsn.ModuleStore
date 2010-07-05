using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class Qualified<T>: SqlToken where T: SqlName {
		private readonly T name;
		private readonly SqlName qualification;

		[Rule("<ColumnNameQualified> ::= <ColumnName>", typeof(ColumnName))]
		[Rule("<ColumnWildQualified> ::= <ColumnWild>", typeof(ColumnName))]
		[Rule("<ProcedureNameQualified> ::= <ProcedureName>", typeof(ProcedureName))]
		[Rule("<TableNameQualified> ::= <TableName>", typeof(TableName))]
		[Rule("<TypeNameQualified> ::= <TypeName>", typeof(TypeName))]
		public Qualified(T name): this(null, name) {}

		[Rule("<ColumnNameQualified> ::= <TableName> '.' <ColumnName>", typeof(ColumnName), ConstructorParameterMapping = new[] {0, 2})]
		[Rule("<ColumnNameQualified> ::= <VariableName> '.' <ColumnName>", typeof(ColumnName), ConstructorParameterMapping = new[] {0, 2})]
		[Rule("<ColumnWildQualified> ::= <TableName> '.' <ColumnWild>", typeof(ColumnName), ConstructorParameterMapping = new[] {0, 2})]
		[Rule("<ColumnWildQualified> ::= <VariableName> '.' <ColumnWild>", typeof(ColumnName), ConstructorParameterMapping = new[] {0, 2})]
		[Rule("<ProcedureNameQualified> ::= <SchemaName> '.' <ProcedureName>", typeof(ProcedureName), ConstructorParameterMapping = new[] {0, 2})]
		[Rule("<TableNameQualified> ::= <SchemaName> '.' <TableName>", typeof(TableName), ConstructorParameterMapping = new[] {0, 2})]
		[Rule("<TypeNameQualified> ::= <SchemaName> '.' <TypeName>", typeof(TypeName), ConstructorParameterMapping = new[] {0, 2})]
		public Qualified(SqlName qualification, T name) {
			if (name == null) {
				throw new ArgumentNullException("name");
			}
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

		public SqlName Qualification {
			get {
				return qualification;
			}
		}
	}
}