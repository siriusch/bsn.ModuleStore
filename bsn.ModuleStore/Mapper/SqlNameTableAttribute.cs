using System;

namespace bsn.ModuleStore.Mapper {
	/// <summary>
	/// The DbNameTableAttribute attribute is used to specify the XML nametable to use for XML-based return values.
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
	public sealed class SqlNameTableAttribute: Attribute {}
}