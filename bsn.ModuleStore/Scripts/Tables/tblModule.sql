CREATE TABLE [tblModule] (
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
)