CREATE PROCEDURE [spModuleUpdate]
    @uidModule [uniqueidentifier],
    @sAssemblyName [nvarchar](250),
    @binSetupHash [binary](64),
    @iUpdateVersion [int]
AS
    BEGIN
        SET NOCOUNT ON;
        UPDATE [tblModule] SET [sAssemblyName]=@sAssemblyName, [binSetupHash]=@binSetupHash, [iUpdateVersion]=@iUpdateVersion WHERE (@uidModule=[tblModule].[uidModule]) AND ([iUpdateVersion]<=@iUpdateVersion);
        RETURN CAST(@@ROWCOUNT AS [int]);
    END