using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using bsn.ModuleStore;

[assembly: SqlSetupScript("Scripts.Indexes.IX_tblModule_Cluster.sql")]
[assembly: SqlSetupScript("Scripts.Indexes.IX_tblModule_uidAssembly.sql")]
[assembly: SqlSetupScript("Scripts.Procedures.spModuleAdd.sql")]
[assembly: SqlSetupScript("Scripts.Procedures.spModuleDelete.sql")]
[assembly: SqlSetupScript("Scripts.Procedures.spModuleList.sql")]
[assembly: SqlSetupScript("Scripts.Procedures.spModuleUpdate.sql")]
[assembly: SqlSetupScript("Scripts.Tables.tblModule.sql")]
[assembly: SqlSetupScript("Scripts.Views.vwModule.sql")]
