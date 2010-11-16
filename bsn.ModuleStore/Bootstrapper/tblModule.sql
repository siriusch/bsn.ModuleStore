CREATE TABLE [dbo].[tblModule] (
    [uidModule] uniqueidentifier ROWGUIDCOL NOT NULL CONSTRAINT [DF_tblModule_uidModule] DEFAULT ((NEWID())),
    [uidAssemblyGuid] uniqueidentifier NOT NULL,
    [sSchema] sysname COLLATE Latin1_General_CI_AS NOT NULL,
    [sAssemblyName] nvarchar(250) COLLATE Latin1_General_CI_AS NOT NULL,
    [binSetupHash] varbinary(64) NULL,
    [iUpdateVersion] int NOT NULL CONSTRAINT [DF_tblModule_iUpdateVersion] DEFAULT ((0)),
    [dtSetup] datetime NOT NULL CONSTRAINT [DF_tblModule_dtSetup] DEFAULT ((GETUTCDATE())),
    [dtUpdate] datetime NULL,
    CONSTRAINT [PK_tblModule] PRIMARY KEY NONCLUSTERED (
        [uidModule] ASC
    ) WITH (PAD_INDEX=OFF, STATISTICS_NORECOMPUTE=OFF, IGNORE_DUP_KEY=OFF, ALLOW_ROW_LOCKS=ON, ALLOW_PAGE_LOCKS=ON)
);

CREATE CLUSTERED INDEX [IX_tblModule_Cluster] ON [dbo].[tblModule] (
    [dtSetup] ASC
) WITH (PAD_INDEX=OFF, STATISTICS_NORECOMPUTE=OFF, IGNORE_DUP_KEY=OFF, ALLOW_ROW_LOCKS=ON, ALLOW_PAGE_LOCKS=ON);

CREATE NONCLUSTERED INDEX [IX_tblModule_uidAssembly] ON [dbo].[tblModule] (
    [uidAssemblyGuid] ASC
) WITH (PAD_INDEX=OFF, STATISTICS_NORECOMPUTE=OFF, IGNORE_DUP_KEY=OFF, ALLOW_ROW_LOCKS=ON, ALLOW_PAGE_LOCKS=ON);
