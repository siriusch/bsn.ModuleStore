CREATE PROCEDURE [spModuleList]
    @uidAssemblyGuid [uniqueidentifier]
AS
    BEGIN
        SET NOCOUNT ON;
        SELECT *
        FROM [vwModule] AS [m]
        WHERE (@uidAssemblyGuid IS NULL) OR (@uidAssemblyGuid=[m].[uidAssemblyGuid]);
    END