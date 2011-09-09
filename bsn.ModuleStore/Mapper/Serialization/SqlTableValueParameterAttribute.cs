using System;

using Microsoft.SqlServer.Server;

namespace bsn.ModuleStore.Mapper.Serialization {
	[AttributeUsage(AttributeTargets.Property|AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class SqlTableValueParameterAttribute : Attribute {
		internal string GetClrUserDefinedTypeName(Type type, SqlTableValueParameterAttribute tvpAttribute)
		{
			string schemaName = ((tvpAttribute != null) && !String.IsNullOrEmpty(tvpAttribute.SchemaName)) ? tvpAttribute.SchemaName.Replace("[", "").Replace("]", "") : "dbo";
			Type clrType = type.IsNullableType() ? type.GetGenericArguments()[0] : type;
			SqlUserDefinedTypeAttribute[] attributes = (SqlUserDefinedTypeAttribute[])clrType.GetCustomAttributes(typeof(SqlUserDefinedTypeAttribute), false);
			if (attributes.Length > 0) {
				string typeName = attributes[0].Name.Replace("[", "").Replace("]", "");
				return String.Format("[{0}].[{1}]", schemaName, typeName);
			}
			return String.Empty;
		}

		/// <summary>
		/// Gets or sets the index.
		/// </summary>
		/// <value>The index.</value>
		public int Index {
			get;
			set;
		}

		/// <summary>
		/// The name for the database binding.
		/// </summary>
		public string SchemaName {
			get;
			set;
		}
	}
}
