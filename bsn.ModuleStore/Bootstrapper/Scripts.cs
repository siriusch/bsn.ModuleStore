using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using bsn.ModuleStore;

[assembly: SqlSetupScript("Bootstrapper.IX_tblModule_Cluster.sql")]
[assembly: SqlSetupScript("Bootstrapper.IX_tblModule_uidAssembly.sql")]
[assembly: SqlSetupScript("Bootstrapper.tblModule.sql")]
[assembly: SqlSetupScript("Bootstrapper.vwModule.sql")]
