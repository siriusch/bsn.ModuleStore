﻿using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class Qualified<T>: SqlScriptableToken where T: SqlName {
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

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(Qualification, WhitespacePadding.None, null, ".");
			writer.WriteScript(Name, WhitespacePadding.None);
		}
	}
}