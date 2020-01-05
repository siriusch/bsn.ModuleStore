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

namespace bsn.ModuleStore.Mapper.AssemblyMetadata {
	internal class SqlCallProcedureInfo: IQualified<SchemaName> {
		private static readonly Regex rxUncomment = new Regex(@"(?<=^/\*\s*)\S.*?(?=\s*\*/$)|(?<=^--\s*)\S.*?(?=\s*$)", RegexOptions.CultureInvariant|RegexOptions.ExplicitCapture|RegexOptions.Singleline);
		private static readonly Dictionary<string, CreateProcedureStatement> statements = new Dictionary<string, CreateProcedureStatement>(StringComparer.Ordinal);

		public static CreateProcedureStatement GetCreateScript(Type type, string manifestResourceName) {
			if (type == null) {
				throw new ArgumentNullException(nameof(type));
			}
			if (manifestResourceName == null) {
				throw new ArgumentNullException(nameof(manifestResourceName));
			}
			manifestResourceName = type.Namespace+Type.Delimiter+manifestResourceName;
			CreateProcedureStatement result;
			var manifestResourceKey = type.Assembly.FullName+", "+manifestResourceName;
			lock (statements) {
				if (!statements.TryGetValue(manifestResourceKey, out result)) {
					using (var stream = type.Assembly.GetManifestResourceStream(manifestResourceName)) {
						if (stream != null) {
							using (TextReader reader = new StreamReader(stream, true)) {
								using (var enumerator = ScriptParser.Parse(reader).OfType<CreateProcedureStatement>().GetEnumerator()) {
									if (!enumerator.MoveNext()) {
										throw new InvalidOperationException($"No CREATE PROCEDURE statement found in script {manifestResourceName}.");
									}
									result = enumerator.Current;
									Debug.Assert(result != null);
									if (enumerator.MoveNext()) {
										throw new InvalidOperationException($"More than one CREATE PROCEDURE statement was found in script {manifestResourceName}");
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
		private readonly ISerializationTypeInfo returnTypeInfo;
		private readonly CreateProcedureStatement script;
		private readonly bool useReturnValue;
		private readonly ParameterInfo xmlNameTableParameter;
		private SchemaName schemaNameOverride;

		public SqlCallProcedureInfo(AssemblyInventory inventory, ISerializationTypeInfoProvider serializationTypeInfoProvider, MethodInfo method, ISerializationTypeMappingProvider typeMappingProvider) {
			foreach (SqlProcedureAttribute attribute in method.GetCustomAttributes(typeof(SqlProcedureAttribute), false)) {
				proc = attribute;
			}
			if (proc == null) {
				throw new NotImplementedException($"The method {method.DeclaringType.FullName}.{method.Name} has no SQL script attached to it.");
			}
			script = GetCreateScript(proc.ManifestResourceType ?? method.DeclaringType, proc.ManifestResourceName);
			if (script == null) {
				throw new FileNotFoundException($"The embedded script for the method {method.DeclaringType.FullName}.{method.Name} could not be found", proc.ManifestResourceName);
			}
			if (proc.ExecuteFirstCommentBeforeInvocation && (script.Comments.Count > 0)) {
				var callStatements = new List<Statement>(ScriptParser.Parse(rxUncomment.Match(script.Comments.First()).Value));
				if (callStatements.Count > 0) {
					foreach (var preCallStatement in callStatements) {
						foreach (var qualifiedName in preCallStatement.GetObjectSchemaQualifiedNames(script.ObjectSchema)) {
							qualifiedName.SetOverride(this);
						}
					}
					preCallStatements = callStatements.ToArray();
				}
			}
			parameters = new SqlCallParameterBase[script.Parameters.Count];
			foreach (SqlParameterAttribute parameterAttribute in method.GetCustomAttributes(typeof(SqlParameterAttribute), false)) {
				var parameterName = parameterAttribute.ParameterName;
				if (string.IsNullOrEmpty(parameterName)) {
					throw new InvalidOperationException($"Constant parameters on method {method.DeclaringType.FullName}.{method.Name} must specify a valid name");
				}
				if (!parameterName.StartsWith("@")) {
					parameterName = "@"+parameterName;
				}
				var found = false;
				for (var i = 0; i < script.Parameters.Count; i++) {
					var parameter = script.Parameters[i];
					if (string.Equals(parameter.ParameterName.Value, parameterName, StringComparison.OrdinalIgnoreCase)) {
						if (parameters[i] != null) {
							throw new InvalidOperationException($"The constant parameter {parameterName} on method {method.DeclaringType.FullName}.{method.Name} cannot be declared multiple times");
						}
						parameters[i] = new SqlCallParameterConstant(script.ProcedureName.Name, parameter, parameterAttribute.Value);
						found = true;
						break;
					}
				}
				if (!found) {
					throw new InvalidOperationException($"The constant parameter {parameterName} on method {method.DeclaringType.FullName}.{method.Name} does not match any of the stored procedure parameter names");
				}
			}
			var index = 0;
			var methodParameters = method.GetParameters();
			foreach (var parameterInfo in methodParameters.OrderBy(p => p.Position)) {
				if ((parameterInfo.GetCustomAttributes(typeof(SqlNameTableAttribute), true).Length > 0) || (typeof(XmlNameTable).IsAssignableFrom(parameterInfo.ParameterType))) {
					if (xmlNameTableParameter != null) {
						throw new InvalidOperationException($"Only one XML name table parameter is allowed for method {method.DeclaringType.FullName}.{method.Name}");
					}
					xmlNameTableParameter = parameterInfo;
				} else {
					while ((index < parameters.Length) && (parameters[index] != null)) {
						index++;
					}
					if (index >= parameters.Length) {
						throw new InvalidOperationException($"The method {method.DeclaringType.FullName}.{method.Name} has more parameters than its stored procedure");
					}
					var callParameterInfo = new SqlCallParameterInfo(serializationTypeInfoProvider, parameterInfo, script.ProcedureName.Name, script.Parameters[index], typeMappingProvider);
					if (callParameterInfo.Direction != ParameterDirection.Input) {
						outArgCount = methodParameters.Length;
					}
					parameters[index] = callParameterInfo;
				}
			}
			if (parameters.Any(p => p == null)) {
				throw new InvalidOperationException($"The method {method.DeclaringType.FullName}.{method.Name} has less parameters than its stored procedure");
			}
			returnTypeInfo = serializationTypeInfoProvider.GetSerializationTypeInfo(method.ReturnType, proc.UseReturnValue==SqlReturnValue.Scalar);
			if ((proc.UseReturnValue != SqlReturnValue.Auto) || (method.ReturnType != typeof(void))) {
				useReturnValue = (proc.UseReturnValue == SqlReturnValue.ReturnValue) || ((proc.UseReturnValue == SqlReturnValue.Auto) && (typeMappingProvider.GetMapping(method.ReturnType).DbType == SqlDbType.Int));
			}
			exceptionMappings = inventory.ExceptionMappings.Where(a => (a.DeclaredOn == null) || (a.DeclaredOn == method.DeclaringType) || (a.DeclaredOn == method)).OrderByDescending(a => a.ComputeSpecificity()).ToArray();
		}

		public string ProcedureName => script.ProcedureName.Name.Value;

		public ICollection<SqlCommand> GetCommands(IMethodCallMessage mcm, SqlConnection connection, string schemaName, out SqlParameter returnParameter, out SqlParameter[] outArgs, out ISerializationTypeInfo returnTypeInfo, out ICallDeserializationInfo procInfo, out XmlNameTable xmlNameTable,
		                                           IList<IDisposable> disposeList) {
			if (String.IsNullOrEmpty(schemaName)) {
				throw new ArgumentNullException(nameof(schemaName));
			}
			VerifyArgs(mcm);
			SqlCommand[] commands;
			if (preCallStatements != null) {
				commands = new SqlCommand[preCallStatements.Length+1];
				lock (preCallStatements) {
					schemaNameOverride = new SchemaName(schemaName);
					var index = 0;
					foreach (var preCallStatement in preCallStatements) {
						var output = new StringWriter(CultureInfo.InvariantCulture);
						var writer = new SqlWriter(output, DatabaseEngine.Unknown, SqlWriterMode.NoComments);
						preCallStatement.WriteTo(writer);
						var preCallCommand = connection.CreateCommand();
						preCallCommand.CommandType = CommandType.Text;
						preCallCommand.CommandText = output.ToString();
						commands[index++] = preCallCommand;
					}
				}
			} else {
				commands = new SqlCommand[1];
			}
			var callCommand = connection.CreateCommand();
			callCommand.CommandType = CommandType.StoredProcedure;
			callCommand.CommandText = $"[{schemaName}].[{ProcedureName}]";
			if (proc.Timeout > 0) {
				callCommand.CommandTimeout = proc.Timeout;
			}
			outArgs = new SqlParameter[outArgCount];
			xmlNameTable = xmlNameTableParameter != null ? (XmlNameTable)mcm.GetArg(xmlNameTableParameter.Position) : null;
			foreach (var factory in parameters) {
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
			foreach (var mapping in exceptionMappings) {
				if (mapping.GetNumber(ex.Number).Equals(ex.Number) && mapping.GetSeverity(ex.Class).Equals(ex.Class) && mapping.GetState(ex.State).Equals(ex.State)) {
					return (Exception)Activator.CreateInstance(mapping.TargetException, mapping.GetMessage(ex.Message), ex);
				}
			}
			return ex;
		}

		[Conditional("DEBUG")]
		private void VerifyArgs(IMethodCallMessage mcm) {
			var expectedNames = parameters.OfType<SqlCallParameterInfo>().Select(p => p.ParameterName).ToList();
			if (xmlNameTableParameter != null) {
				expectedNames.Insert(xmlNameTableParameter.Position, xmlNameTableParameter.Name);
			}
			if (mcm.ArgCount != expectedNames.Count) {
				throw new InvalidOperationException($"{mcm.MethodBase.DeclaringType.FullName}.{mcm.MethodName} parameter verification failed, the parameter count does not match");
			}
			for (var i = 0; i < expectedNames.Count; i++) {
				if (!string.Equals(expectedNames[i], mcm.GetArgName(i), StringComparison.Ordinal)) {
					throw new InvalidOperationException($"{mcm.MethodBase.DeclaringType.FullName}.{mcm.MethodName} parameter {i} verification failed, expected {expectedNames[i]} but got {mcm.GetArgName(i)}");
				}
			}
		}

		SchemaName IQualified<SchemaName>.Qualification => schemaNameOverride;
	}
}
