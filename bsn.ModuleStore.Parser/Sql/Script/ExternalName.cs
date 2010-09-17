using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ExternalName: SqlScriptableToken {
		private readonly SqlAssemblyName assemblyName;
		private readonly ClassName className;
		private readonly MethodName methodName;

		[Rule("<ExternalName> ::= ~EXTERNAL_NAME <AssemblyName> ~'.' <ClassName> ~'.' <MethodName>")]
		public ExternalName(SqlAssemblyName assemblyName, ClassName className, MethodName methodName) {
			Debug.Assert(assemblyName != null);
			Debug.Assert(className != null);
			Debug.Assert(methodName != null);
			this.assemblyName = assemblyName;
			this.className = className;
			this.methodName = methodName;
		}

		public SqlAssemblyName AssemblyName {
			get {
				return assemblyName;
			}
		}

		public ClassName ClassName {
			get {
				return className;
			}
		}

		public MethodName MethodName {
			get {
				return methodName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("EXTERNAL NAME ");
			writer.WriteScript(assemblyName, WhitespacePadding.None);
			writer.Write(".");
			writer.WriteScript(className, WhitespacePadding.None);
			writer.Write(".");
			writer.WriteScript(methodName, WhitespacePadding.None);
		}
	}
}
