CREATE PROCEDURE [schema].[spModuleAdd]
    @uidModule [uniqueidentifier],
    @uidAssemblyGuid [uniqueidentifier],
    @sSchemaPrefix [sysname],
    @sAssemblyName [nvarchar](250)
AS
    BEGIN
        SET NOCOUNT ON;
        SET @uidModule=COALESCE(@uidModule, NEWID());
        WITH [ExistingSchema] AS (
            SELECT COALESCE([m].[sSchema], [iss].[SCHEMA_NAME]) AS [sSchemaName]
            FROM [schema].[tblModule] AS [m]
            FULL JOIN [INFORMATION_SCHEMA].[SCHEMATA] AS [iss] ON [m].[sSchema]=[iss].[SCHEMA_NAME]
        ),
        [SchemaName] AS (
            SELECT 1 AS [iInstance], @sSchemaPrefix AS [sSchemaName]
            UNION ALL 
            SELECT [sn].[iInstance]+1, CAST(@sSchemaPrefix+'_'+CAST([sn].[iInstance]+1 AS [varchar]) AS [sysname])
            FROM [SchemaName] AS [sn]
            JOIN [ExistingSchema] AS [es] ON [es].[sSchemaName]=[sn].[sSchemaName]
        )
        INSERT INTO [schema].[tblModule] ([uidModule], [uidAssemblyGuid], [sSchema], [sAssemblyName])
        SELECT TOP (1) @uidModule, @uidAssemblyGuid, [sn].[sSchemaName], @sAssemblyName
        FROM [SchemaName] AS [sn]
        ORDER BY [sn].[iInstance] DESC;
        SELECT *
        FROM [vwModule] AS [m]
        WHERE [m].[uidModule]=@uidModule;
    END;
