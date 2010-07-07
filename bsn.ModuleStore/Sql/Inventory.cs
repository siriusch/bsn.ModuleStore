// (C) 2010 Arsène von Wyss / bsn
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using bsn.ModuleStore.Sql.Script;

namespace bsn.ModuleStore.Sql {
	[Serializable]
	public class Inventory: ISerializable {
		private readonly Dictionary<string, SqlToken> objects = new Dictionary<string, SqlToken>();

		public Inventory() {}

		protected Inventory(SerializationInfo info, StreamingContext context) {
			if (info == null) {
				throw new ArgumentNullException("info");
			}
			objects = (Dictionary<string, SqlToken>)info.GetValue("objects", typeof(Dictionary<string, SqlToken>));
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