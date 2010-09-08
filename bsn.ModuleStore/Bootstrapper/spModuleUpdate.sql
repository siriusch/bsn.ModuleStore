CREATE PROCEDURE [schema].[spModuleUpdate]
    @uidModule [uniqueidentifier],
    @sAssemblyName [nvarchar](250),
    @binSetupHash [binary](64),
    @iUpdateVersion [int]
AS
    BEGIN
        SET NOCOUNT ON;
        UPDATE [schema].[tblModule] SET [sAssemblyName]=@sAssemblyName, [binSetupHash]=@binSetupHash, [iUpdateVersion]=@iUpdateVersion, [dtUpdate]=GETUTCDATE() WHERE (@uidModule=[tblModule].[uidModule]) AND ([iUpdateVersion]<=@iUpdateVersion);
        RETURN CAST(@@ROWCOUNT AS [int]);
    END;
