using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script.Tokens {
	[Terminal("(EOF)")]
	[Terminal("(Error)")]
	[Terminal("(Whitespace)")]
	[Terminal("(Comment End)")]
	[Terminal("(Comment Line)")]
	[Terminal("(Comment Start)")]
	[Terminal("(")]
	[Terminal(")")]
	[Terminal(".")]
	[Terminal(",")]
	[Terminal(";")]
	[Terminal("_RETURNS")]
	[Terminal("ALL")]
	[Terminal("AS")]
	[Terminal("ASC")]
	[Terminal("BEGIN")]
	[Terminal("BEGIN_CATCH")]
	[Terminal("BEGIN_TRY")]
	[Terminal("BETWEEN")]
	[Terminal("BREAK")]
	[Terminal("BROWSE")]
	[Terminal("BY")]
	[Terminal("CALLED_ON_NULL_INPUT")]
	[Terminal("CASCADE")]
	[Terminal("CASE")]
	[Terminal("CAST_")]
	[Terminal("CHECK")]
	[Terminal("CLOSE")]
	[Terminal("COLLATE")]
	[Terminal("COLUMN")]
	[Terminal("CONSTRAINT")]
	[Terminal("CONTINUE")]
	[Terminal("COUNT_")]
	[Terminal("CROSS")]
	[Terminal("CURSOR")]
	[Terminal("DEALLOCATE")]
	[Terminal("DECLARE")]
	[Terminal("DEFAULT")]
	[Terminal("DESC")]
	[Terminal("DISABLE_TRIGGER")]
	[Terminal("DISTINCT")]
	[Terminal("ELSE")]
	[Terminal("ENABLE_TRIGGER")]
	[Terminal("END")]
	[Terminal("END_CATCH")]
	[Terminal("END_TRY")]
	[Terminal("ESCAPE")]
	[Terminal("EXECUTE")]
	[Terminal("EXISTS")]
	[Terminal("FOR")]
	[Terminal("FOR_UPDATE")]
	[Terminal("FOREIGN")]
	[Terminal("FROM")]
	[Terminal("FULL")]
	[Terminal("FULLTEXT_INDEX")]
	[Terminal("FUNCTION")]
	[Terminal("GOTO")]
	[Terminal("GROUP")]
	[Terminal("HAVING")]
	[Terminal("IDENTITY")]
	[Terminal("IDENTITY_INSERT")]
	[Terminal("IF")]
	[Terminal("IN")]
	[Terminal("INCLUDE_")]
	[Terminal("INDEX")]
	[Terminal("INNER")]
	[Terminal("INSTEAD_OF")]
	[Terminal("INTO")]
	[Terminal("IS")]
	[Terminal("JOIN")]
	[Terminal("KEY")]
	[Terminal("LEFT")]
	[Terminal("LIKE")]
	[Terminal("NO_ACTION")]
	[Terminal("NO_POPULATION")]
	[Terminal("NOCHECK")]
	[Terminal("OF")]
	[Terminal("OPEN")]
	[Terminal("OPENXML")]
	[Terminal("ORDER")]
	[Terminal("OUTER")]
	[Terminal("OVER")]
	[Terminal("PARTITION_BY")]
	[Terminal("PRINT")]
	[Terminal("PROCEDURE")]
	[Terminal("PUBLIC")]
	[Terminal("RAISERROR")]
	[Terminal("READ_ONLY")]
	[Terminal("REFERENCES")]
	[Terminal("REPLICATION")]
	[Terminal("RETURN")]
	[Terminal("RETURNS_NULL_ON_NULL_INPUT")]
	[Terminal("RIGHT")]
	[Terminal("ROWGUIDCOL")]
	[Terminal("SET")]
	[Terminal("TABLE")]
	[Terminal("THEN")]
	[Terminal("TOP")]
	[Terminal("TRIGGER")]
	[Terminal("TYPE_COLUMN")]
	[Terminal("UNION")]
	[Terminal("USING_XML_INDEX")]
	[Terminal("VALUES")]
	[Terminal("VIEW")]
	[Terminal("WAITFOR")]
	[Terminal("WHEN")]
	[Terminal("WHERE")]
	[Terminal("WHILE")]
	[Terminal("WITH")]
	[Terminal("WITH_CHANGE_TRACKING")]
	[Terminal("WITH_FILLFACTOR")]
	[Terminal("XML_INDEX")]
	[Terminal("XML_SCHEMA_COLLECTION")]
	public class InsignificantToken: SqlToken {
		[Rule("<OptionalAs> ::= AS", AllowTruncationForConstructor = true)]
		[Rule("<OptionalAs> ::=")]
		[Rule("<Terminator> ::= ';'", AllowTruncationForConstructor = true)]
		[Rule("<Terminator> ::= <Terminator> ';'", AllowTruncationForConstructor = true)]
		[Rule("<OptionalInto> ::=")]
		[Rule("<OptionalInto> ::= INTO", AllowTruncationForConstructor = true)]
		[Rule("<OptionalFrom> ::=")]
		[Rule("<OptionalFrom> ::= FROM", AllowTruncationForConstructor = true)]
		public InsignificantToken() {}
	}
}