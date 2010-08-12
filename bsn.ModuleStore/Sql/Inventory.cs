// (C) 2010 Arsène von Wyss / bsn
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using bsn.ModuleStore.Sql.Script;

namespace bsn.ModuleStore.Sql {
	[Serializable]
	public class Inventory: ISerializable {
		private readonly Dictionary<string, SqlScriptableToken> objects = new Dictionary<string, SqlScriptableToken>();

		public Inventory() {}

		protected Inventory(SerializationInfo info, StreamingContext context) {
			if (info == null) {
				throw new ArgumentNullException("info");
			}
			objects = (Dictionary<string, SqlScriptableToken>)info.GetValue("objects", typeof(Dictionary<string, SqlScriptableToken>));
		}

		protected virtual void GetObjectData(SerializationInfo info, StreamingContext context) {
			if (info == null) {
				throw new ArgumentNullException("info");
			}
			info.AddValue("objects", objects);
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) {
			GetObjectData(info, context);
		}
	}
}