// bsn ModuleStore database versioning
// -----------------------------------
// 
// Copyright 2010 by Arsène von Wyss - avw@gmx.ch
// 
// Development has been supported by Sirius Technologies AG, Basel
// 
// Source:
// 
// https://bsn-modulestore.googlecode.com/hg/
// 
// License:
// 
// The library is distributed under the GNU Lesser General Public License:
// http://www.gnu.org/licenses/lgpl.html
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text.RegularExpressions;
using System.Xml;

using bsn.ModuleStore.Mapper.Serialization;
using bsn.ModuleStore.Sql;
using bsn.ModuleStore.Sql.Script;

namespace bsn.ModuleStore.Mapper {
	internal class SqlCallProcedureInfo: IQualified<SchemaName> {
		private static readonly Regex rxUncomment = new Regex(@"(?<=^/\*\s*)\S.*?(?=\s*\*/$)|(?<=^--\s*)\S.*?(?=\s*$)", RegexOptions.CultureInvariant|RegexOptions.ExplicitCapture|RegexOptions.Singleline);
		private static readonly Dictionary<string, CreateProcedureStatement> statements = new Dictionary<string, CreateProcedureStatement>(StringComparer.Ordinal);

		public static CreateProcedureStatement GetCreateScript(Type type, string manifestResourceName) {
			if (type == null) {
				throw new ArgumentNullException("type");
			}
			if (manifestResourceName == null) {
				throw new ArgumentNullException("manifestResourceName");
			}
			manifestResourceName = type.Namespace+Type.Delimiter+manifestResourceName;
			CreateProcedureStatement result;
			string manifestResourceKey = type.Assembly.FullName+", "+manifestResourceName;
			lock (statements) {
				if (!statements.TryGetValue(manifestResourceKey, out result)) {
					using (Stream stream = type.Assembly.GetManifestResourceStream(manifestResourceName)) {
						if (stream != null) {
							using (TextReader reader = new StreamReader(stream, true)) {
								using (IEnumerator<CreateProcedureStatement> enumerator = ScriptParser.Parse(reader).OfType<CreateProcedureStatement>().GetEnumerator()) {
									if (!enumerator.MoveNext()) {
										throw new InvalidOperationException(String.Format("No CREATE PROCEDURE statement found in script {0}.", manifestResourceName));
									}
									result = enumerator.Current;
									Debug.Assert(result != null);
									if (enumerator.MoveNext()) {
										throw new InvalidOperationException(String.Format("More than one CREATE PROCEDURE statement was found in script {0}", manifestResourceName));
									}
								}
							}
						}
					}
					statements.Add(manifestResourceKey, result);
				}
			}
			return result;
		}

		private readonly IEnumerable<SqlExceptionMappingAttribute> exceptionMappings;

		private readonly int outArgCount;
		private readonly SqlCallParameterBase[] parameters;
		private readonly Statement[] preCallStatements;
		private readonly SqlProcedureAttribute proc;
		private readonly SqlSerializationTypeInfo returnTypeInfo;
		private readonly CreateProcedureStatement script;
		private readonly bool useReturnValue;
		private readonly ParameterInfo xmlNameTableParameter;
		private SchemaName schemaNameOverride;

		public SqlCallProcedureInfo(AssemblyInventory inventory, MethodInfo method) {
			foreach (SqlProcedureAttribute attribute in method.GetCustomAttributes(typeof(SqlProcedureAttribute), false)) {
				proc = attribute;
			}
			if (proc == null) {
				throw new NotImplementedException(String.Format("The method {0}.{1} has no SQL script attached to it.", method.DeclaringType.FullName, method.Name));
			}
			script = GetCreateScript(proc.ManifestResourceType ?? method.DeclaringType, proc.ManifestResourceName);
			if (script == null) {
				throw new FileNotFoundException(String.Format("The embedded script for the method {0}.{1} could not be found", method.DeclaringType.FullName, method.Name), proc.ManifestResourceName);
			}
			if (proc.ExecuteFirstCommentBeforeInvocation && (script.Comments.Count > 0)) {
				List<Statement> preCallStatements = new List<Statement>(ScriptParser.Parse(rxUncomment.Match(script.Comments.First()).Value));
				if (preCallStatements.Count > 0) {
					foreach (Statement preCallStatement in preCallStatements) {
						foreach (IQualifiedName<SchemaName> qualifiedName in preCallStatement.GetObjectSchemaQualifiedNames(script.ObjectSchema)) {
							qualifiedName.SetOverride(this);
						}
					}
					this.preCallStatements = preCallStatements.ToArray();
				}
			}
			parameters = new SqlCallParameterBase[script.Parameters.Count];
			foreach (SqlParameterAttribute parameterAttribute in method.GetCustomAttributes(typeof(SqlParameterAttribute), false)) {
				string parameterName = parameterAttribute.ParameterName;
				if (string.IsNullOrEmpty(parameterName)) {
					throw new InvalidOperationException(string.Format("Constant parameters on method {0}.{1} must specify a valid name", method.DeclaringType.FullName, method.Name));
				}
				if (!parameterName.StartsWith("@")) {
					parameterName = "@"+parameterName;
				}
				bool found = false;
				for (int i = 0; i < script.Parameters.Count; i++) {
					ProcedureParameter parameter = script.Parameters[i];
					if (string.Equals(parameter.ParameterName.Value, parameterName, StringComparison.OrdinalIgnoreCase)) {
						if (parameters[i] != null) {
							throw new InvalidOperationException(string.Format("The constant parameter {0} on method {1}.{2} cannot be declared multiple times", parameterName, method.DeclaringType.FullName, method.Name));
						}
						parameters[i] = new SqlCallParameterConstant(parameter, parameterAttribute.Value);
						found = true;
						break;
					}
				}
				if (!found) {
					throw new InvalidOperationException(string.Format("The constant parameter {0} on method {1}.{2} does not match any of the stored procedure parameter names", parameterName, method.DeclaringType.FullName, method.Name));
				}
			}
			int index = 0;
			ParameterInfo[] methodParameters = method.GetParameters();
			foreach (ParameterInfo parameterInfo in methodParameters.OrderBy(p => p.Position)) {
				if ((parameterInfo.GetCustomAttributes(typeof(SqlNameTableAttribute), true).Length > 0) || (typeof(XmlNameTable).IsAssignableFrom(parameterInfo.ParameterType))) {
					if (xmlNameTableParameter != null) {
						throw new InvalidOperationException(string.Format("Only one XML name table parameter is allowed for method {0}.{1}", method.DeclaringType.FullName, method.Name));
					}
					xmlNameTableParameter = parameterInfo;
				} else {
					while ((index < parameters.Length) && (parameters[index] != null)) {
						index++;
					}
					if (index >= parameters.Length) {
						throw new InvalidOperationException(String.Format("The method {0}.{1} has more parameters than its stored procedure", method.DeclaringType.FullName, method.Name));
					}
					SqlCallParameterInfo callParameterInfo = new SqlCallParameterInfo(parameterInfo, script.Parameters[index]);
					if (callParameterInfo.Direction != ParameterDirection.Input) {
						outArgCount = methodParameters.Length;
					}
					parameters[index] = callParameterInfo;
				}
			}
			if (parameters.Any(p => p == null)) {
				throw new InvalidOperationException(String.Format("The method {0}.{1} has less parameters than its stored procedure", method.DeclaringType.FullName, method.Name));
			}
			returnTypeInfo = SqlSerializationTypeInfo.Get(method.ReturnType);
			if ((proc.UseReturnValue != SqlReturnValue.Auto) || (method.ReturnType != typeof(void))) {
				useReturnValue = (proc.UseReturnValue == SqlReturnValue.ReturnValue) || ((proc.UseReturnValue == SqlReturnValue.Auto) && (SqlSerializationTypeMapping.GetTypeMapping(method.ReturnType) == SqlDbType.Int));
			}
			exceptionMappings = inventory.ExceptionMappings.Where(a => (a.DeclaredOn == null) || (a.DeclaredOn == method.DeclaringType) || (a.DeclaredOn == method)).OrderByDescending(a => a.ComputeSpecificity()).ToArray();
		}

		public string ProcedureName {
			get {
				return script.ProcedureName.Name.Value;
			}
		}

		public ICollection<SqlCommand> GetCommands(IMethodCallMessage mcm, SqlConnection connection, string schemaName, out SqlParameter returnParameter, out SqlParameter[] outArgs, out SqlSerializationTypeInfo returnTypeInfo, out ICallDeserializationInfo procInfo, out XmlNameTable xmlNameTable,
		                                           IList<IDisposable> disposeList) {
			if (String.IsNullOrEmpty(schemaName)) {
				throw new ArgumentNullException("schemaName");
			}
			VerifyArgs(mcm);
			SqlCommand[] commands;
			if (preCallStatements != null) {
				commands = new SqlCommand[preCallStatements.Length+1];
				lock (preCallStatements) {
					schemaNameOverride = new SchemaName(schemaName);
					int index = 0;
					foreach (Statement preCallStatement in preCallStatements) {
						StringWriter output = new StringWriter(CultureInfo.InvariantCulture);
						SqlWriter writer = new SqlWriter(output, DatabaseEngine.Unknown, false);
						preCallStatement.WriteTo(writer);
						SqlCommand preCallCommand = connection.CreateCommand();
						preCallCommand.CommandType = CommandType.Text;
						preCallCommand.CommandText = output.ToString();
						commands[index++] = preCallCommand;
					}
				}
			} else {
				commands = new SqlCommand[1];
			}
			SqlCommand callCommand = connection.CreateCommand();
			callCommand.CommandType = CommandType.StoredProcedure;
			callCommand.CommandText = String.Format("[{0}].[{1}]", schemaName, ProcedureName);
			if (proc.Timeout > 0) {
				callCommand.CommandTimeout = proc.Timeout;
			}
			outArgs = new SqlParameter[outArgCount];
			xmlNameTable = xmlNameTableParameter != null ? (XmlNameTable)mcm.GetArg(xmlNameTableParameter.Position) : null;
			foreach (SqlCallParameterBase factory in parameters) {
				callCommand.Parameters.Add(factory.GetSqlParameter(callCommand, mcm, outArgs, disposeList));
			}
			if (useReturnValue) {
				returnParameter = callCommand.CreateParameter();
				returnParameter.SqlDbType = SqlDbType.Int;
				returnParameter.Direction = ParameterDirection.ReturnValue;
				callCommand.Parameters.Add(returnParameter);
			} else {
				returnParameter = null;
			}
			returnTypeInfo = this.returnTypeInfo;
			procInfo = proc;
			commands[commands.Length-1] = callCommand;
			return commands;
		}

		public Exception MapException(SqlException ex) {
			foreach (SqlExceptionMappingAttribute mapping in exceptionMappings) {
				if (mapping.MessageId.GetValueOrDefault(ex.Number).Equals(ex.Number) && mapping.Severity.GetValueOrDefault(ex.Class).Equals(ex.Class) && mapping.State.GetValueOrDefault(ex.State).Equals(ex.State)) {
					return (Exception)Activator.CreateInstance(mapping.TargetException, ex.Message, ex);
				}
			}
			return ex;
		}

		[Conditional("DEBUG")]
		private void VerifyArgs(IMethodCallMessage mcm) {
			List<string> expectedNames = parameters.OfType<SqlCallParameterInfo>().Select(p => p.ParameterName).ToList();
			if (xmlNameTableParameter != null) {
				expectedNames.Insert(xmlNameTableParameter.Position, xmlNameTableParameter.Name);
			}
			if (mcm.ArgCount != expectedNames.Count) {
				throw new InvalidOperationException(string.Format("{0}.{1} parameter verification failed, the parameter count does not match", mcm.MethodBase.DeclaringType.FullName, mcm.MethodName));
			}
			for (int i = 0; i < expectedNames.Count; i++) {
				if (!string.Equals(expectedNames[i], mcm.GetArgName(i), StringComparison.Ordinal)) {
					throw new InvalidOperationException(string.Format("{0}.{1} parameter {2} verification failed, expected {3} but got {4}", mcm.MethodBase.DeclaringType.FullName, mcm.MethodName, i, expectedNames[i], mcm.GetArgName(i)));
				}
			}
		}

		SchemaName IQualified<SchemaName>.Qualification {
			get {
				return schemaNameOverride;
			}
		}
	}
}
