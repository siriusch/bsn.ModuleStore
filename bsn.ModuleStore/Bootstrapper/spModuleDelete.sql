CREATE PROCEDURE [schema].[spModuleDelete]
    @uidModule [uniqueidentifier]
AS
    BEGIN
        SET NOCOUNT ON;
        DELETE FROM [schema].[tblModule] WHERE [tblModule].[uidModule]=@uidModule;
        RETURN CAST(@@ROWCOUNT AS [int]);
    END;
