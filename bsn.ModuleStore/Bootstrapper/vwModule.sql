CREATE VIEW [vwModule] AS
    SELECT [m].[uidModule], [m].[uidAssemblyGuid], [m].[sSchema], [m].[sAssemblyName], [m].[binSetupHash], [m].[iUpdateVersion], [m].[dtSetup], [m].[dtUpdate], CAST(CASE
        WHEN [iss].[SCHEMA_NAME] IS NOT NULL THEN 1
        ELSE 0
        END AS [bit]) AS [fSchemaExists]
    FROM [tblModule] AS [m]
    LEFT JOIN [INFORMATION_SCHEMA].[SCHEMATA] AS [iss] ON [m].[sSchema]=[iss].[SCHEMA_NAME]