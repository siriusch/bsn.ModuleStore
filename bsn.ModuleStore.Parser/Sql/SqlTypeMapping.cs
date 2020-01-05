using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace bsn.ModuleStore.Sql {
	public sealed class SqlTypeMapping {
		private static readonly Dictionary<Type, IList<SqlTypeMapping>> byClrType = new Dictionary<Type, IList<SqlTypeMapping>>();
		private static readonly Dictionary<SqlDbType, IList<SqlTypeMapping>> byDbType = new Dictionary<SqlDbType, IList<SqlTypeMapping>>(Enum.GetValues(typeof(SqlDbType)).Length);
		private static readonly Dictionary<string, IList<SqlTypeMapping>> byName = new Dictionary<string, IList<SqlTypeMapping>>(StringComparer.OrdinalIgnoreCase);

		static SqlTypeMapping() {
			using (var stream = typeof(SqlTypeMapping).Assembly.GetManifestResourceStream(typeof(SqlTypeMapping), "SqlTypes.xml")) {
				Debug.Assert(stream != null);
				using (var reader = XmlReader.Create(stream)) {
					var doc = XDocument.Load(reader);
					Debug.Assert(doc.Root != null);
					foreach (var mapping in doc.Root.Elements("type").Select(e => new SqlTypeMapping(e))) {
						foreach (var name in mapping.Names) {
							Append(byName, name, mapping);
						}
						foreach (var clrType in mapping.Types) {
							Append(byClrType, clrType, mapping);
						}
						if (mapping.SqlType.HasValue) {
							Append(byDbType, mapping.SqlType.Value, mapping);
						}
					}
				}
				Finalize(byName);
				Finalize(byClrType);
				Finalize(byDbType);
			}
		}

		private static void Append<T>(Dictionary<T, IList<SqlTypeMapping>> items, T key, SqlTypeMapping value) {
			if (!items.TryGetValue(key, out var list)) {
				list = new List<SqlTypeMapping>(1);
				items.Add(key, list);
			}
			list.Add(value);
		}

		private static void Finalize<T>(Dictionary<T, IList<SqlTypeMapping>> items) {
			foreach (var key in items.Keys.ToList()) {
				var list = (List<SqlTypeMapping>)items[key];
				list.Sort((x, y) => y.Engine-x.Engine);
				items[key] = list.ToArray();
			}
		}

		public static SqlTypeMapping Get(string name, DatabaseEngine engineVersion) {
			if (name == null) {
				throw new ArgumentNullException(nameof(name));
			}
			return Get(byName, name, engineVersion);
		}

		public static SqlTypeMapping Get(Type clrType, DatabaseEngine engineVersion) {
			if (clrType == null) {
				throw new ArgumentNullException(nameof(clrType));
			}
			return Get(byClrType, clrType, engineVersion);
		}

		public static SqlTypeMapping Get(SqlDbType dbType, DatabaseEngine engineVersion) {
			return Get(byDbType, dbType, engineVersion);
		}

		private static SqlTypeMapping Get<T>(Dictionary<T, IList<SqlTypeMapping>> items, T key, DatabaseEngine engineVersion) {
			if (items.TryGetValue(key, out var typeMappings)) {
				return typeMappings.FirstOrDefault(m => m.Engine <= engineVersion);
			}
			return null;
		}

		public static bool IsBuiltinTypeName(string name) {
			if (name == null) {
				throw new ArgumentNullException(nameof(name));
			}
			return byName.ContainsKey(name);
		}

		internal static bool TryGetBuiltinTypeName(ref string name) {
			if (byName.TryGetValue(name, out var typeMappings)) {
				name = typeMappings.First().Name;
				return true;
			}
			return false;
		}

		private readonly DatabaseEngine engine;
		private readonly ReadOnlyCollection<string> names;
		private readonly SqlDbType? sqlType;
		private readonly ReadOnlyCollection<Type> types;

		private SqlTypeMapping(XElement element) {
			if (element == null) {
				throw new ArgumentNullException(nameof(element));
			}
			var nameAttribute = element.Attribute("name");
			if (nameAttribute == null) {
				throw new ArgumentException("A name is required", nameof(element));
			}
			var attribute = element.Attribute("alias");
			names = Array.AsReadOnly(attribute != null ? new[] {nameAttribute.Value, attribute.Value} : new[] {nameAttribute.Value});
			attribute = element.Attribute("dbType");
			if (attribute != null) {
				sqlType = (SqlDbType)Enum.Parse(typeof(SqlDbType), attribute.Value);
			}
			attribute = element.Attribute("engine");
			if (attribute != null) {
				engine = (DatabaseEngine)XmlConvert.ToInt32(attribute.Value);
			}
			types = Array.AsReadOnly(element.Elements("clr").Attributes("name").Select(a => Type.GetType(a.Value, true, false)).ToArray());
		}

		public DatabaseEngine Engine => engine;

		public string Name => names[0];

		public ReadOnlyCollection<string> Names => names;

		public SqlDbType? SqlType => sqlType;

		public ReadOnlyCollection<Type> Types => types;
	}
}
