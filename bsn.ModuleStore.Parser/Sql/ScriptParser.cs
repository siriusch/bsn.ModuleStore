// (C) 2010 Arsène von Wyss / bsn
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using bsn.GoldParser.Grammar;
using bsn.GoldParser.Parser;
using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script;

namespace bsn.ModuleStore.Sql {
	public static class ScriptParser {
		private static readonly Dictionary<string, string> builtinFunctions = LoadFromFile("Sql2005Functions.txt");
		private static readonly Dictionary<string, string> builtinTypes = LoadFromFile("Sql2005Types.txt");
		// reserved word list: http://msdn.microsoft.com/en-us/library/aa238507(v=SQL.80).aspx
		private static readonly Dictionary<string, string> reservedWords = LoadFromFile("Sql2005Keywords.txt");
		private static readonly object sync = new object();
		private static CompiledGrammar compiledGrammar;
		private static SemanticTypeActions<SqlToken> semanticActions;
		// functions: http://technet.microsoft.com/en-us/library/ms174318(SQL.90).aspx

		private static Dictionary<string, string> LoadFromFile(string filename) {
			Dictionary<string, string> result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			using (Stream stream = typeof(ScriptParser).Assembly.GetManifestResourceStream(typeof(ScriptParser), filename)) {
				Debug.Assert(stream != null);
				using (TextReader reader = new StreamReader(stream, true)) {
					for (string line = reader.ReadLine(); line != null; line = reader.ReadLine()) {
						line = line.Trim();
						if (line.Length > 0) {
							try {
								result.Add(line, line);
							} catch (ArgumentException) {
								Debug.WriteLine(string.Format("Duplicate entry {0} in file {1}", line, filename));
							}
						}
					}
				}
			}
			return result;
		}

		internal static CompiledGrammar GetGrammar() {
			lock (sync) {
				if (compiledGrammar == null) {
					compiledGrammar = CompiledGrammar.Load(typeof(ScriptParser), "ModuleStoreSQL.cgt");
				}
				return compiledGrammar;
			}
		}

		public static SemanticTypeActions<SqlToken> GetSemanticActions() {
			lock (sync) {
				if (semanticActions == null) {
					semanticActions = new SemanticTypeActions<SqlToken>(GetGrammar());
					semanticActions.Initialize();
				}
				return semanticActions;
			}
		}

		public static bool IsReservedWord(string name) {
			return reservedWords.ContainsKey(name);
		}

		internal static bool TryGetReservedWord(ref string word) {
			string result;
			if (reservedWords.TryGetValue(word, out result)) {
				word = result;
				return true;
			}
			return false;
		}

		public static bool IsBuiltinFunctionName(string functionName) {
			return builtinFunctions.ContainsKey(functionName);
		}

		internal static bool TryGetBuiltinFunctionName(ref string name) {
			string result;
			if (builtinFunctions.TryGetValue(name, out result)) {
				name = result;
				return true;
			}
			return false;
		}

		public static IEnumerable<Statement> Parse(string sql) {
			using (StringReader reader = new StringReader(sql)) {
				return Parse(reader);
			}
		}

		public static IEnumerable<Statement> Parse(TextReader sql) {
			ParseMessage parseMessage = ParseMessage.None;
			SqlSemanticProcessor processor = new SqlSemanticProcessor(sql, GetSemanticActions());
			try {
				parseMessage = processor.ParseAll();
			} catch (Exception ex) {
				throw new ParseException(ex.Message, parseMessage, ((IToken)processor.CurrentToken).Position, ex);
			}
			if (parseMessage != ParseMessage.Accept) {
				throw new ParseException("The supplied SQL could not be parsed", parseMessage, ((IToken)processor.CurrentToken).Position);
			}
			return (IEnumerable<Statement>)processor.CurrentToken;
		}

		public static bool IsBuiltinTypeName(string typeName) {
			return builtinTypes.ContainsKey(typeName);
		}

		internal static bool TryGetBuiltinTypeName(ref string name) {
			string result;
			if (builtinTypes.TryGetValue(name, out result)) {
				name = result;
				return true;
			}
			return false;
		}
	}
}