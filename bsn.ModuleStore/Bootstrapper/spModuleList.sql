CREATE PROCEDURE [dbo].[spModuleList]
    @uidAssemblyGuid uniqueidentifier
AS
    BEGIN
        SET NOCOUNT ON;
        SELECT *
            FROM [dbo].[vwModule] AS [m]
            WHERE (@uidAssemblyGuid IS NULL) OR (@uidAssemblyGuid=[m].[uidAssemblyGuid]);
    END;
