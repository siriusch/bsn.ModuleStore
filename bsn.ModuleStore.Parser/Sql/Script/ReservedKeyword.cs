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

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("AS")]
	[Terminal("AUTHORIZATION")]
	[Terminal("BACKUP")]
	[Terminal("BEGIN")]
	[Terminal("BETWEEN")]
	[Terminal("BREAK")]
	[Terminal("BROWSE")]
	[Terminal("BULK")]
	[Terminal("BY")]
	[Terminal("CASCADE")]
	[Terminal("CASE")]
	[Terminal("CHECK")]
	[Terminal("CHECKPOINT")]
	[Terminal("CLOSE")]
	[Terminal("COLLATE")]
	[Terminal("COLUMN")]
	[Terminal("COMMIT")]
	[Terminal("COMPUTE")]
	[Terminal("CONSTRAINT")]
	[Terminal("CONTAINS")]
	[Terminal("CONTAINSTABLE")]
	[Terminal("CONTINUE")]
	[Terminal("CROSS")]
	[Terminal("CURRENT")]
	[Terminal("CURRENT_DATE")]
	[Terminal("CURRENT_TIME")]
	[Terminal("CURRENT_TIMESTAMP")]
	[Terminal("CURRENT_USER")]
	[Terminal("CURSOR")]
	[Terminal("DATABASE")]
	[Terminal("DBCC")]
	[Terminal("DEALLOCATE")]
	[Terminal("DECLARE")]
	[Terminal("DEFAULT")]
	[Terminal("DENY")]
	[Terminal("DISK")]
	[Terminal("DISTRIBUTED")]
	[Terminal("DOUBLE")]
	[Terminal("DUMMY")]
	[Terminal("DUMP")]
	[Terminal("ELSE")]
	[Terminal("END")]
	[Terminal("ERRLVL")]
	[Terminal("ESCAPE")]
	[Terminal("EXCEPT")]
	[Terminal("EXECUTE")]
	[Terminal("EXISTS")]
	[Terminal("EXIT")]
	[Terminal("FETCH")]
	[Terminal("FILE")]
	[Terminal("FOR")]
	[Terminal("FOREIGN")]
	[Terminal("FREETEXT")]
	[Terminal("FREETEXTTABLE")]
	[Terminal("FROM")]
	[Terminal("FULL")]
	[Terminal("FUNCTION")]
	[Terminal("GOTO")]
	[Terminal("GRANT")]
	[Terminal("GROUP")]
	[Terminal("HAVING")]
	[Terminal("IDENTITY")]
	[Terminal("IDENTITY_INSERT")]
	[Terminal("IDENTITYCOL")]
	[Terminal("IF")]
	[Terminal("IN")]
	[Terminal("INDEX")]
	[Terminal("INNER")]
	[Terminal("INTERSECT")]
	[Terminal("INTO")]
	[Terminal("IS")]
	[Terminal("JOIN")]
	[Terminal("KEY")]
	[Terminal("KILL")]
	[Terminal("LEFT")]
	[Terminal("LIKE")]
	[Terminal("LINENO")]
	[Terminal("LOAD")]
	[Terminal("MERGE")]
	[Terminal("NATIONAL")]
	[Terminal("NOCHECK")]
	[Terminal("NULLIF")]
	[Terminal("OF")]
	[Terminal("OFFSETS")]
	[Terminal("OPEN")]
	[Terminal("OPENDATASOURCE")]
	[Terminal("OPENQUERY")]
	[Terminal("OPENROWSET")]
	[Terminal("OPENXML")]
	[Terminal("OPTION")]
	[Terminal("ORDER")]
	[Terminal("OUTER")]
	[Terminal("OVER")]
	[Terminal("PLAN")]
	[Terminal("PRINT")]
	[Terminal("PROCEDURE")]
	[Terminal("PUBLIC")]
	[Terminal("RAISERROR")]
	[Terminal("READ")]
	[Terminal("READTEXT")]
	[Terminal("RECONFIGURE")]
	[Terminal("REFERENCES")]
	[Terminal("REPLICATION")]
	[Terminal("RESTORE")]
	[Terminal("RESTRICT")]
	[Terminal("RETURN")]
	[Terminal("REVOKE")]
	[Terminal("RIGHT")]
	[Terminal("ROLLBACK")]
	[Terminal("ROWCOUNT")]
	[Terminal("RULE")]
	[Terminal("SAVE")]
	[Terminal("SCHEMA")]
	[Terminal("SESSION_USER")]
	[Terminal("SET")]
	[Terminal("SETUSER")]
	[Terminal("SHUTDOWN")]
	[Terminal("STATISTICS")]
	[Terminal("SYSTEM_USER")]
	[Terminal("TABLE")]
	[Terminal("TEXTSIZE")]
	[Terminal("THEN")]
	[Terminal("TO")]
	[Terminal("TOP")]
	[Terminal("TRANSACTION")]
	[Terminal("TRIGGER")]
	[Terminal("TRUNCATE")]
	[Terminal("TSEQUAL")]
	[Terminal("UNION")]
	[Terminal("UPDATETEXT")]
	[Terminal("USE")]
	[Terminal("USER")]
	[Terminal("VALUES")]
	[Terminal("VIEW")]
	[Terminal("WAITFOR")]
	[Terminal("WHEN")]
	[Terminal("WHERE")]
	[Terminal("WHILE")]
	[Terminal("WITH")]
	[Terminal("WRITETEXT")]
	public sealed class ReservedKeyword: KeywordToken {
		public ReservedKeyword(string keyword): base(keyword) {}
	}
}
