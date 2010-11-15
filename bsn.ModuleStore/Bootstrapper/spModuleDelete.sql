CREATE PROCEDURE [dbo].[spModuleDelete]
    @uidModule uniqueidentifier
AS
    BEGIN
        SET NOCOUNT ON;
        DELETE
            FROM [dbo].[tblModule] WHERE [tblModule].[uidModule]=@uidModule;
        RETURN CAST(@@ROWCOUNT AS int);
    END;
