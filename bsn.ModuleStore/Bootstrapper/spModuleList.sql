CREATE PROCEDURE [schema].[spModuleList]
    @uidAssemblyGuid [uniqueidentifier]
AS
    BEGIN
        SET NOCOUNT ON;
        SELECT *
        FROM [schema].[vwModule] AS [m]
        WHERE (@uidAssemblyGuid IS NULL) OR (@uidAssemblyGuid=[m].[uidAssemblyGuid]);
    END;
