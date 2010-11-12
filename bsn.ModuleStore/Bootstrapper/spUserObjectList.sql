CREATE PROCEDURE [dbo].[spUserObjectList]
	@sSchema sysname
AS
	BEGIN
		DECLARE @tblObject TABLE (
			[iObject] int NOT NULL PRIMARY KEY,
			[sSchema] sysname NOT NULL,
			[sObject] sysname NOT NULL, 
			[sType] varchar(2) NOT NULL,
			[sDefinition] nvarchar(MAX)
		);
		-- Gather object information first
		INSERT @tblObject
			SELECT [o].[object_id], [s].[name], [o].[name], [o].[type], OBJECT_DEFINITION([o].[object_id])
			FROM [sys].[objects] AS [o]
			JOIN [sys].[schemas] AS [s] ON [o].[schema_id]=[s].[schema_id]
			LEFT JOIN [sys].[extended_properties] AS [mdts] ON ([mdts].[name]='microsoft_database_tools_support') AND ([mdts].[class]='1') AND (CONVERT(bit, [mdts].[value])=1) AND ([o].[object_id]=[mdts].[major_id])
			WHERE ([mdts].[class] IS NULL) AND ([o].[type] IN ('AF', 'FN', 'FS', 'FT', 'IF', 'P', 'PC', 'TA', 'TF', 'TR', 'U', 'V')) AND ((@sSchema IS NULL) OR (@sSchema=[s].[name]));
		-- Send object definitions to the client
		SELECT *
			FROM @tblObject;
		-- Send index definition to the client
		SELECT [o].[iObject], [i].[name] AS [sIndex], CONVERT(int, [i].[type]) AS [iType], CONVERT(bit, [i].[is_unique]) AS [bUnique], CONVERT(bit, [i].[ignore_dup_key]) AS [bIgnoreDupKey], CONVERT(bit, [i].[is_primary_key]) AS [bPrimaryKey], CONVERT(bit, [i].[is_unique_constraint]) AS [bUniqueConstraint], [i].[fill_factor] AS [iFillFactor], (
				SELECT *
					FROM (
						SELECT [ic].[index_column_id] AS [ix], [c].[name] AS [columnName], [ic].[key_ordinal] AS [keyOrdinal], [ic].[is_descending_key] AS [isDescendingKey], [ic].[is_included_column] AS [isIncludedColumn]
							FROM [sys].[index_columns] AS [ic]
							JOIN [sys].[columns] AS [c] ON ([ic].[column_id] = [c].[column_id]) AND ([o].[iObject] = [c].[object_id])
							WHERE ([ic].[object_id] = [o].[iObject]) AND ([ic].[index_id] = [i].[index_id])
					) AS [column]
					ORDER BY [column].[ix]
					FOR XML AUTO, TYPE, ROOT('columns') 
			) AS [xDefinition]
			FROM [sys].[indexes] [i]
			JOIN @tblObject [o] ON [i].[object_id] = [o].[iObject];
		-- Finally send XML Schema Collections to the client
		SELECT [s].[name] AS [sSchema], [x].[name] AS [sXmlSchemaCollectionName], XML_SCHEMA_NAMESPACE([s].[name], [x].[name]) AS [xDefinition]
			FROM [sys].[xml_schema_collections] AS [x]
			JOIN [sys].[schemas] AS [s] ON [x].[schema_id]=[s].[schema_id]
			WHERE ([s].[name] <> 'sys') AND ((@sSchema IS NULL) OR (@sSchema=[s].[name]));
	END;
