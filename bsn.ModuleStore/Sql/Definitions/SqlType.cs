// (C) 2010 Arsène von Wyss / bsn
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace bsn.ModuleStore.Sql.Definitions {
	/// <summary>
	/// A SQL data type, such as "int" or "uniqueidentifier"
	/// </summary>
	[Serializable]
	public class SqlType: Metadata<SqlType> {
		private enum SqlTypeInfo {
			None = 0,
			Length = 1,
			Decimal = 2
		}

		private static readonly Dictionary<string, KeyValuePair<string, SqlTypeInfo>> builtInTypeInfo;
		private static readonly Dictionary<string, SqlType> builtInTypes;

		static SqlType() {
			builtInTypeInfo = new Dictionary<string, KeyValuePair<string, SqlTypeInfo>>(NameComparer);
			builtInTypes = new Dictionary<string, SqlType>(ReferenceEqualityComparer<string>.Default);
			// fixed-size get their singleton
			foreach (
					string typeName in
							new[]
								{
										"bigint", "int", "real", "float", "smallint", "tinyint", "bit", "money", "smallmoney", "time", "date", "smalldatetime", "datetime", "datetime2", "datetimeoffset", "text", "ntext", "image",
										"timestamp", "uniqueidentifier", "xml", "sql_variant"
								}) {
				builtInTypeInfo.Add(typeName, new KeyValuePair<string, SqlTypeInfo>(typeName, SqlTypeInfo.None));
				builtInTypes.Add(typeName, new SqlType(typeName));
			}
			// variable-sized only get registered
			foreach (string typeName in new[] {"char", "nchar", "varchar", "nvarchar", "binary", "varbinary"}) {
				builtInTypeInfo.Add(typeName, new KeyValuePair<string, SqlTypeInfo>(typeName, SqlTypeInfo.Length));
				builtInTypes.Add(typeName, null);
			}
			// aliases for decimal
			KeyValuePair<string, SqlTypeInfo> decimalInfo = new KeyValuePair<string, SqlTypeInfo>("decimal", SqlTypeInfo.Decimal);
			builtInTypeInfo.Add("decimal", decimalInfo);
			builtInTypeInfo.Add("dec", decimalInfo);
			builtInTypeInfo.Add("numeric", decimalInfo);
			builtInTypes.Add(decimalInfo.Key, null);
		}

		public static string UnifyName(string name) {
			if (string.IsNullOrEmpty(name)) {
				throw new ArgumentNullException("name");
			}
			KeyValuePair<string, SqlTypeInfo> typeInfo;
			if (builtInTypeInfo.TryGetValue(name, out typeInfo)) {
				return typeInfo.Key;
			}
			return name;
		}
		
		public static SqlType UnifyType(SqlType type) {
			if (type == null) {
				throw new ArgumentNullException("type");
			}
			if (type.GetType()==typeof(SqlType)) {
				SqlType result;
				if (builtInTypes.TryGetValue(type.Name, out result)) {
					return result;
				}
			}
			return type;
		}

		public static SqlType Create(string name, int? lengthOrPrecision, int? scale) {
			KeyValuePair<string, SqlTypeInfo> typeInfo;
			if (builtInTypeInfo.TryGetValue(name, out typeInfo)) {
				if (scale.HasValue && (!(typeInfo.Value == SqlTypeInfo.Decimal))) {
					throw new ArgumentException("Scale supplied for a type which does not support it", "scale");
				}
				switch (typeInfo.Value) {
				case SqlTypeInfo.None:
					if (lengthOrPrecision.HasValue) {
						if (typeInfo.Key == "float") {
							if (lengthOrPrecision.Value <= 24) {
								typeInfo = builtInTypeInfo["real"];
							}
						} else {
							throw new ArgumentException("Length or precision supplied for a type which does not support it", "lengthOrPrecision");
						}
					}
					SqlType result = builtInTypes[typeInfo.Key];
					Debug.Assert(result != null);
					return result;
				case SqlTypeInfo.Length:
					return new SqlVarType(typeInfo.Key, lengthOrPrecision);
				case SqlTypeInfo.Decimal:
					return new SqlDecType(typeInfo.Key, lengthOrPrecision, scale);
				}
			}
			if (!(lengthOrPrecision.HasValue || scale.HasValue)) {
				return new SqlType(name); // UDT?
			}
			throw new ArgumentException("Unsupported type definition");
		}

		protected SqlType(string name): base(UnifyName(name)) {}
		protected SqlType(SerializationInfo info, StreamingContext context): base(info, context, UnifyName) {}
		
		public bool IsBuiltIn {
			get {
				return builtInTypes.ContainsKey(Name);
			}
		}
	}
}