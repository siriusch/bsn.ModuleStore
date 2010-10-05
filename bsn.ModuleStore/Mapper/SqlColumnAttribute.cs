using System;
using System.Reflection;

namespace bsn.ModuleStore.Mapper {
	/// <summary>
	/// The DbColumnAttribute is used to change the binding name on <see cref="ITypedDataReader"/> interfaces, or to specify the fields to be deserialized when the <see cref="DbDeserializer"/> is used.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property|AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public sealed class SqlColumnAttribute: Attribute {
		/// <summary>
		/// Get a single DbColumnAttribute instance.
		/// </summary>
		/// <param name="info">The <see cref="MemberInfo"/> to query for the DbColumnAttribute attribute.</param>
		/// <param name="autoCreate">If true, a DbColumnAttribute is inferred from the MemberInfo when no attribute is found. Otherwise, null is returned in this situation.</param>
		/// <returns>The DbColumnAttribute for the member.</returns>
		public static SqlColumnAttribute GetColumnAttribute(MemberInfo info, bool autoCreate) {
			if (info == null) {
				throw new ArgumentNullException("info");
			}
			SqlColumnAttribute[] columnAttributes = (SqlColumnAttribute[])info.GetCustomAttributes(typeof(SqlColumnAttribute), true);
			if (columnAttributes.Length > 0) {
				SqlColumnAttribute result = columnAttributes[0];
				if (string.IsNullOrEmpty(result.Name)) {
					result = result.CloneWithName(info.Name);
				}
				return result;
			}
			if (autoCreate) {
				return new SqlColumnAttribute(info.Name);
			}
			return null;
		}

		private DateTimeKind dateTimeKind = DateTimeKind.Unspecified;
		private string name;

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlColumnAttribute"/> class.
		/// </summary>
		public SqlColumnAttribute(): this(null) {}

		/// <summary>
		/// Create a new DbColumnAttribute.
		/// </summary>
		/// <param name="name">The DB column name to bind to.</param>
		public SqlColumnAttribute(string name): base() {
			this.name = name;
		}

		/// <summary>
		/// Gets or sets the kind of the DateTime, if the instance is one.
		/// </summary>
		/// <value>The kind of the date time.</value>
		public DateTimeKind DateTimeKind {
			get {
				return dateTimeKind;
			}
			set {
				dateTimeKind = value;
			}
		}

		/// <summary>
		/// The name for the database binding.
		/// </summary>
		public string Name {
			get {
				return name;
			}
		}

		internal SqlColumnAttribute CloneWithName(string newName) {
			SqlColumnAttribute result = (SqlColumnAttribute)MemberwiseClone();
			result.name = newName;
			return result;
		}
	}
}