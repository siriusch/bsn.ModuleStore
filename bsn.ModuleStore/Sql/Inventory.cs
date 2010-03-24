// (C) 2010 Arsène von Wyss / bsn
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using bsn.ModuleStore.Sql.Definitions;

namespace bsn.ModuleStore.Sql {
	[Serializable]
	public class Inventory: ISerializable {
		private readonly Dictionary<string, SqlObject> objects = new Dictionary<string, SqlObject>();

		public Inventory() {}

		protected Inventory(SerializationInfo info, StreamingContext context) {
			if (info == null) {
				throw new ArgumentNullException("info");
			}
			objects = (Dictionary<string, SqlObject>)info.GetValue("objects", typeof(Dictionary<string, SqlObject>));
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) {
			GetObjectData(info, context);
		}

		protected virtual void GetObjectData(SerializationInfo info, StreamingContext context) {
			if (info == null) {
				throw new ArgumentNullException("info");
			}
			info.AddValue("objects", objects);
		}
	}
}