SELECT [s].[name] AS [sSchema], [o].[name] AS [sName], 
    -- Types 'AF', 'FS', 'FT', 'PC', 'TA' are not yet implemented!
		CASE [o].[type] 
    WHEN 'FN' THEN
			CONVERT(xml, (SELECT '['+[s].[name]+'].['+[o].[name]+']' AS [@Name], (
					SELECT 'preserve' AS [@xml:space], OBJECT_DEFINITION([o].[object_id]) FOR XML PATH ('SQL'), TYPE
			) FOR XML PATH ('Function'), TYPE))
    WHEN 'IF' THEN
      CONVERT(xml, (SELECT '['+[s].[name]+'].['+[o].[name]+']' AS [@Name], (
					SELECT 'preserve' AS [@xml:space], OBJECT_DEFINITION([o].[object_id]) FOR XML PATH ('SQL'), TYPE
			) FOR XML PATH ('Function'), TYPE))
    WHEN 'TF' THEN
      CONVERT(xml, (SELECT '['+[s].[name]+'].['+[o].[name]+']' AS [@Name], (
					SELECT 'preserve' AS [@xml:space], OBJECT_DEFINITION([o].[object_id]) FOR XML PATH ('SQL'), TYPE
			) FOR XML PATH ('Function'), TYPE))
	  WHEN 'P' THEN
      CONVERT(xml, (SELECT '['+[s].[name]+'].['+[o].[name]+']' AS [@Name], (
					SELECT 'preserve' AS [@xml:space], OBJECT_DEFINITION([o].[object_id]) FOR XML PATH ('SQL'), TYPE
			) FOR XML PATH ('Procedure'), TYPE))
    WHEN 'V' THEN
			CONVERT(xml, (SELECT '['+[s].[name]+'].['+[o].[name]+']' AS [@Name], (
					SELECT 'preserve' AS [@xml:space], OBJECT_DEFINITION([o].[object_id]) FOR XML PATH ('SQL'), TYPE
			), (
                    SELECT '['+[i].[name]+']' AS [@Name], [i].[type_desc] AS [@Type], NULLIF(CONVERT(bit, [i].[is_unique]), 0) AS [@Unique], NULLIF(CONVERT(bit, [i].[ignore_dup_key]), 0) AS [@IgnoreDuplicateKeys], NULLIF(CONVERT(bit, [i].[is_primary_key]), 0) AS [@PrimaryKey], NULLIF(CONVERT(bit, [i].[is_unique_constraint]), 0) AS [@UniqueConstraint], NULLIF(CONVERT(bit, [i].[allow_page_locks]), 0) AS [@AllowPageLocks], NULLIF(CONVERT(bit, [i].[allow_row_locks]), 0) AS [@AllowRowLocks], NULLIF(CONVERT(bit, [i].[is_padded]), 0) AS [@Padded], NULLIF([i].[fill_factor], 0) AS [@FillFactor], (
                            SELECT '['+[c].[name]+']' AS [@Name], CASE [ic].[is_descending_key]
                                WHEN 0 THEN 'ASC'
                                WHEN 1 THEN 'DESC'
                                END AS [@Order], NULLIF([ic].[is_included_column], 0) AS [@IsIncludedColumn]
                            FROM [sys].[index_columns] AS [ic]
                            JOIN [sys].[columns] AS [c] ON ([ic].[column_id]=[c].[column_id]) AND ([i].[object_id]=[c].[object_id])
                            WHERE ([ic].[object_id]=[i].[object_id]) AND ([ic].[index_id]=[i].[index_id])
                            ORDER BY [ic].[key_ordinal] FOR XML PATH ('Column'), TYPE
                        )
                    FROM [sys].[indexes] AS [i]
                    WHERE ([i].[object_id]=[o].[object_id]) AND ([i].[type]>0)
                    ORDER BY [i].[is_primary_key], [i].[name] FOR XML PATH ('Index'), TYPE
                ) FOR XML PATH ('View'), TYPE
          ))
    WHEN 'TR' THEN
      CONVERT(xml, (SELECT '['+[s].[name]+'].['+[o].[name]+']' AS [@Name], (
					SELECT 'preserve' AS [@xml:space], OBJECT_DEFINITION([o].[object_id]) FOR XML PATH ('SQL'), TYPE
			) FOR XML PATH ('Trigger'), TYPE))
    WHEN 'U' THEN
			CONVERT(xml, (SELECT '['+[s].[name]+'].['+[o].[name]+']' AS [@Name], (
                    SELECT '['+[c].[name]+']' AS [@Name], [cc].[definition] AS [@Definition], [cc].[is_persisted] AS [@Persisted], CASE
                        WHEN [c].[is_computed]=1 THEN NULL
                        WHEN [ct].[is_user_defined]=0 THEN [ct].[NAME]
                        ELSE '['+[cts].[name]+'].['+[ct].[name]+']'
                        END AS [@Type], CASE [cc].[is_persisted]
                        WHEN 0 THEN NULL
                        ELSE [c].[is_nullable]
                        END AS [@Nullable], CASE
                        WHEN ([c].[max_length]=-1) AND (([ct].[name]='varchar') OR ([ct].[name]='nvarchar') OR ([ct].[name]='varbinary')) THEN 'MAX'
                        WHEN ([ct].[name]='varchar') OR ([ct].[name]='varbinary') OR ([ct].[name]='char') OR ([ct].[name]='binary') THEN CONVERT(varchar(4), [c].[max_length])
                        WHEN ([ct].[name]='nchar') OR ([ct].[name]='nvarchar') THEN CONVERT(varchar(4), [c].[max_length]/2)
                        ELSE NULL
                        END AS [@Length], CASE
                        WHEN (([ct].[name]='decimal') OR ([ct].[name]='numeric')) AND (([c].[precision]<>18) OR ([c].[scale]>0)) THEN [c].[precision]
                        WHEN (([ct].[name]='real') OR ([ct].[name]='float')) AND ([c].[precision]<>[ct].[precision]) THEN [c].[precision]
                        ELSE NULL
                        END AS [@Precision], CASE
                        WHEN (([ct].[name]='decimal') OR ([ct].[name]='numeric')) AND ([c].[scale]>0) THEN [c].[scale]
                        ELSE NULL
                        END AS [@Scale], [c].[collation_name] AS [@Collation], CASE [dc].[is_system_named]
                        WHEN 0 THEN '['+[dc].[name]+']'
                        ELSE NULL END AS [@DefaultName], [dc].[definition] AS [@Default], NULLIF(CONVERT(bit, [c].[is_rowguidcol]), 0) AS [@RowGuid], [ic].[seed_value] AS [@IdentitySeed], [ic].[seed_value] AS [@IdentityIncrement]
                    FROM [sys].[columns] AS [c]
                    JOIN [sys].[types] AS [ct] ON [c].[user_type_id]=[ct].[user_type_id]
                    JOIN [sys].[schemas] AS [cts] ON [ct].[schema_id]=[cts].[schema_id]
                    LEFT JOIN [sys].[default_constraints] AS [dc] ON ([c].[default_object_id]=[dc].[object_id])
                    LEFT JOIN [sys].[identity_columns] AS [ic] ON ([c].[object_id]=[ic].[object_id]) AND ([c].[column_id]=[ic].[column_id])
                    LEFT JOIN [sys].[computed_columns] AS [cc] ON ([c].[object_id]=[cc].[object_id]) AND ([c].[column_id]=[Cc].[column_id])
                    WHERE [c].[object_id]=[t].[object_id] FOR XML PATH ('Column'), TYPE
                ), (
                    SELECT '['+[cc].[name]+']' AS [@Name], [cc].[definition] AS [@Definition]
                    FROM [sys].[check_constraints] AS [cc]
                    WHERE [cc].[parent_object_id]=[t].[object_id] FOR XML PATH ('CheckConstraint'), TYPE
                ), (
                    SELECT '['+[fk].[name]+']' AS [@Name], '['+[fs].[name]+'].['+[ft].[name]+']' AS [@Reference], CASE [fk].[delete_referential_action]
                        WHEN 0 THEN NULL
                        ELSE [fk].[delete_referential_action_desc]
                        END AS [@OnDelete], CASE [fk].[update_referential_action]
                        WHEN 0 THEN NULL
                        ELSE [fk].[update_referential_action_desc]
                        END AS [@OnUpdate], (
                            SELECT '['+[pc].[name]+']' AS [@Name], '['+[rc].[name]+']' AS [@Reference]
                            FROM [sys].[foreign_key_columns] AS [fkc]
                            JOIN [sys].[columns] AS [pc] ON ([fkc].[parent_object_id]=[pc].[object_id]) AND ([fkc].[parent_column_id]=[pc].[column_id])
                            JOIN [sys].[columns] AS [rc] ON ([fkc].[referenced_object_id]=[rc].[object_id]) AND ([fkc].[referenced_column_id]=[rc].[column_id])
                            WHERE [fkc].[constraint_object_id]=[fk].[object_id] FOR XML PATH ('Column'), TYPE
                        )
                    FROM [sys].[foreign_keys] AS [fk]
                    JOIN [sys].[tables] AS [ft] ON [fk].[referenced_object_id]=[ft].[object_id]
                    JOIN [sys].[schemas] AS [fs] ON [ft].[schema_id]=[fs].[schema_id]
                    WHERE [fk].[parent_object_id]=[t].[object_id]
                    ORDER BY [fk].[key_index_id] FOR XML PATH ('ForeignKeyConstraint'), TYPE
                ), (
                    SELECT '['+[i].[name]+']' AS [@Name], [i].[type_desc] AS [@Type], NULLIF(CONVERT(bit, [i].[is_unique]), 0) AS [@Unique], NULLIF(CONVERT(bit, [i].[ignore_dup_key]), 0) AS [@IgnoreDuplicateKeys], NULLIF(CONVERT(bit, [i].[is_primary_key]), 0) AS [@PrimaryKey], NULLIF(CONVERT(bit, [i].[is_unique_constraint]), 0) AS [@UniqueConstraint], NULLIF(CONVERT(bit, [i].[allow_page_locks]), 0) AS [@AllowPageLocks], NULLIF(CONVERT(bit, [i].[allow_row_locks]), 0) AS [@AllowRowLocks], NULLIF(CONVERT(bit, [i].[is_padded]), 0) AS [@Padded], NULLIF([i].[fill_factor], 0) AS [@FillFactor], (
                            SELECT '['+[c].[name]+']' AS [@Name], CASE [ic].[is_descending_key]
                                WHEN 0 THEN 'ASC'
                                WHEN 1 THEN 'DESC'
                                END AS [@Order], NULLIF([ic].[is_included_column], 0) AS [@IsIncludedColumn]
                            FROM [sys].[index_columns] AS [ic]
                            JOIN [sys].[columns] AS [c] ON ([ic].[column_id]=[c].[column_id]) AND ([i].[object_id]=[c].[object_id])
                            WHERE ([ic].[object_id]=[i].[object_id]) AND ([ic].[index_id]=[i].[index_id])
                            ORDER BY [ic].[key_ordinal] FOR XML PATH ('Column'), TYPE
                        )
                    FROM [sys].[indexes] AS [i]
                    WHERE ([i].[object_id]=[t].[object_id]) AND ([i].[type]>0)
                    ORDER BY [i].[is_primary_key], [i].[name] FOR XML PATH ('Index'), TYPE
                )
            FROM [sys].[tables] AS [t]
            WHERE [t].[object_id]=[o].[object_id] FOR XML PATH ('Table'), TYPE
        ))
    END AS [xDefinition]
    FROM [sys].[objects] AS [o]
    JOIN [sys].[schemas] AS [s] ON [o].[schema_id]=[s].[schema_id]
    LEFT JOIN [sys].[extended_properties] AS [mdts] ON ([mdts].[name]='microsoft_database_tools_support') AND ([mdts].[class]='1') AND (CONVERT(bit, [mdts].[value])=1) AND ([o].[object_id]=[mdts].[major_id])
    WHERE ([mdts].[class] IS NULL) AND ([o].[type] IN ('AF', 'FN', 'FS', 'FT', 'IF', 'P', 'PC', 'TA', 'TF', 'TR', 'U', 'V')) AND ((@sSchema IS NULL) OR (@sSchema=[s].[name]))
    UNION ALL
    SELECT [s].[name], [x].[name], (
            SELECT '['+[s].[name]+'].['+[x].[name]+']' AS [@Name], xml_schema_namespace([s].[name], [x].[name]) FOR XML PATH ('XmlSchemaCollection'), TYPE
        ) AS [xDefinition]
    FROM [sys].[xml_schema_collections] AS [x]
    JOIN [sys].[schemas] AS [s] ON [x].[schema_id]=[s].[schema_id]
    WHERE ([s].[name]<>'sys') AND ((@sSchema IS NULL) OR (@sSchema=[s].[name]))
