CREATE TABLE [schema].[tblModule] (
    [uidModule] [uniqueidentifier] ROWGUIDCOL NOT NULL CONSTRAINT [DF_tblModule_uidModule] DEFAULT (newid()),
    [uidAssemblyGuid] [uniqueidentifier] NOT NULL,
    [sSchema] [sysname] COLLATE Latin1_General_CI_AS NOT NULL,
    [sAssemblyName] [nvarchar](250) COLLATE Latin1_General_CI_AS NOT NULL,
    [binSetupHash] [binary](64) NULL,
    [iUpdateVersion] [int] NOT NULL CONSTRAINT [DF_tblModule_iUpdateVersion] DEFAULT (0),
    [dtSetup] [datetime] NOT NULL CONSTRAINT [DF_tblModule_dtSetup] DEFAULT (getutcdate()),
    [dtUpdate] [datetime] NULL,
    CONSTRAINT [PK_tblModule] PRIMARY KEY NONCLUSTERED (
        [uidModule] ASC
    ) WITH (PAD_INDEX=OFF, STATISTICS_NORECOMPUTE=OFF, IGNORE_DUP_KEY=OFF, ALLOW_ROW_LOCKS=ON, ALLOW_PAGE_LOCKS=ON)
);

CREATE CLUSTERED INDEX [IX_tblModule_Cluster] ON [schema].[tblModule] (
    [dtSetup] ASC
) WITH (PAD_INDEX=OFF, STATISTICS_NORECOMPUTE=OFF, SORT_IN_TEMPDB=OFF, IGNORE_DUP_KEY=OFF, DROP_EXISTING=OFF, ONLINE=OFF, ALLOW_ROW_LOCKS=ON, ALLOW_PAGE_LOCKS=ON);

CREATE NONCLUSTERED INDEX [IX_tblModule_uidAssembly] ON [schema].[tblModule] (
    [uidAssemblyGuid] ASC
) WITH (PAD_INDEX=OFF, STATISTICS_NORECOMPUTE=OFF, SORT_IN_TEMPDB=OFF, IGNORE_DUP_KEY=OFF, DROP_EXISTING=OFF, ONLINE=OFF, ALLOW_ROW_LOCKS=ON, ALLOW_PAGE_LOCKS=ON);
