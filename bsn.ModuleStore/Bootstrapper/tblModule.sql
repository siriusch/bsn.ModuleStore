CREATE TABLE [dbo].[tblModule] (
    [uidModule] uniqueidentifier NOT NULL CONSTRAINT [DF_tblModule_uidModule] DEFAULT ((NEWID())),
    [uidAssemblyGuid] uniqueidentifier NOT NULL,
    [sSchema] sysname NOT NULL,
    [sAssemblyName] nvarchar(250) COLLATE Latin1_General_CI_AS NOT NULL,
    [binSetupHash] varbinary(64) NULL,
    [iUpdateVersion] int NOT NULL CONSTRAINT [DF_tblModule_iUpdateVersion] DEFAULT ((0)),
    [dtSetup] datetime NOT NULL CONSTRAINT [DF_tblModule_dtSetup] DEFAULT ((GETUTCDATE())),
    [dtUpdate] datetime NULL,
    CONSTRAINT [PK_tblModule] PRIMARY KEY NONCLUSTERED (
        [uidModule] ASC
    ) WITH (STATISTICS_NORECOMPUTE=OFF, IGNORE_DUP_KEY=OFF)
);

CREATE CLUSTERED INDEX [IX_tblModule_Cluster] ON [dbo].[tblModule] (
    [dtSetup] ASC
) WITH (STATISTICS_NORECOMPUTE=OFF, IGNORE_DUP_KEY=OFF);

CREATE NONCLUSTERED INDEX [IX_tblModule_uidAssembly] ON [dbo].[tblModule] (
    [uidAssemblyGuid] ASC
) WITH (STATISTICS_NORECOMPUTE=OFF, IGNORE_DUP_KEY=OFF);
