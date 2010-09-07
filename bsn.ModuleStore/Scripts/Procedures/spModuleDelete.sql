CREATE PROCEDURE [spModuleDelete]
    @uidModule [uniqueidentifier]
AS
    BEGIN
        SET NOCOUNT ON;
        DELETE FROM [tblModule] WHERE [tblModule].[uidModule]=@uidModule;
        RETURN CAST(@@ROWCOUNT AS [int]);
    END