using System;

namespace bsn.ModuleStore.Mapper {
	/// <summary>
	/// The DbName attribute is the abstract base class for <see cref="SqlArgAttribute"/>, <see cref="SqlProcAttribute"/> and <see cref="SqlColumnAttribute"/>.
	/// </summary>
	public abstract class SqlNameAttribute: Attribute {
		private string name;

		/// <summary>
		/// Create a new instance.
		/// </summary>
		/// <param name="name">The name so be used. The name should not be empty.</param>
		protected SqlNameAttribute(string name) {
			this.name = name;
		}

		/// <summary>
		/// The name for the database binding.
		/// </summary>
		public string Name {
			get {
				return name;
			}
		}

		internal SqlNameAttribute CloneWithName(string newName) {
			SqlNameAttribute result = (SqlNameAttribute)MemberwiseClone();
			result.name = newName;
			return result;
		}
	}
}