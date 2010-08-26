using System;
using System.Diagnostics;

using bsn.GoldParser.Parser;
using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class Qualified<TQ, TN>: SqlScriptableToken, IQualifiedName<TQ> where TQ: SqlName where TN: SqlName {
		private readonly TN name;
		private TQ qualification;

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

		[Rule("<ColumnNameQualified> ::= <TableName> '.' <ColumnName>", typeof(SqlName), typeof(ColumnName), ConstructorParameterMapping = new[] {0, 2})]
		[Rule("<ColumnNameQualified> ::= <VariableName> '.' <ColumnName>", typeof(SqlName), typeof(ColumnName), ConstructorParameterMapping = new[] {0, 2})]
		[Rule("<ColumnWildQualified> ::= <TableName> '.' <ColumnWild>", typeof(SqlName), typeof(ColumnName), ConstructorParameterMapping = new[] {0, 2})]
		[Rule("<ColumnWildQualified> ::= <VariableName> '.' <ColumnWild>", typeof(SqlName), typeof(ColumnName), ConstructorParameterMapping = new[] {0, 2})]
		[Rule("<ProcedureNameQualified> ::= <SchemaName> '.' <ProcedureName>", typeof(SchemaName), typeof(ProcedureName), ConstructorParameterMapping = new[] {0, 2})]
		[Rule("<FunctionNameQualified> ::= <SchemaName> '.' <FunctionName>", typeof(SchemaName), typeof(FunctionName), ConstructorParameterMapping = new[] {0, 2})]
		[Rule("<TableNameQualified> ::= <SchemaName> '.' <TableName>", typeof(SchemaName), typeof(TableName), ConstructorParameterMapping = new[] {0, 2})]
		[Rule("<TypeNameQualified> ::= <SchemaName> '.' <TypeName>", typeof(SchemaName), typeof(TypeName), ConstructorParameterMapping = new[] {0, 2})]
		[Rule("<ViewNameQualified> ::= <SchemaName> '.' <ViewName>", typeof(SchemaName), typeof(ViewName), ConstructorParameterMapping = new[] {0, 2})]
		[Rule("<XmlSchemaCollectionNameQualified> ::= <SchemaName> '.' <XmlSchemaCollectionName>", typeof(SchemaName), typeof(XmlSchemaCollectionName), ConstructorParameterMapping = new[] {0, 2})]
		[Rule("<TriggerNameQualified> ::= <SchemaName> '.' <TriggerName>", typeof(SchemaName), typeof(TriggerName), ConstructorParameterMapping = new[] {0, 2})]
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

		public TN Name {
			get {
				return name;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			bool isQualified = qualification != null;
			if (isQualified) {
				qualification.WriteToInternal(writer, true);
				writer.Write('.');
			}
			name.WriteToInternal(writer, isQualified);
		}

		internal void SetPosition(LineInfo position) {
			Initialize(((IToken)name).Symbol, position);
		}

		public bool IsQualified {
			get {
				return qualification != null;
			}
		}

		SqlName IQualifiedName<TQ>.Name {
			get {
				return Name;
			}
		}

		public TQ Qualification {
			get {
				return qualification;
			}
			set {
				qualification = value;
			}
		}
	}
}