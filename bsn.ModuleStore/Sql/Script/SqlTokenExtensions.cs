using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace bsn.ModuleStore.Sql.Script {
	public static class SqlTokenExtensions {
		public static T[] ToArray<T>(this Optional<Sequence<T>> sequence) where T: SqlToken {
			if (!sequence.HasValue()) {
				return new T[0];
			}
			return ToArray(sequence.Value);
		}

		public static T[] ToArray<T>(this Sequence<T> sequence) where T: SqlToken {
			return ToList(sequence).ToArray();
		}

		public static List<T> ToList<T>(this Optional<Sequence<T>> sequence) where T: SqlToken {
			if (!sequence.HasValue()) {
				return new List<T>(0);
			}
			return ToList(sequence.Value);
		}

		public static List<T> ToList<T>(this Sequence<T> sequence) where T: SqlToken {
			List<T> result = new List<T>();
			if (sequence != null) {
				result.AddRange(sequence);
			}
			return result;
		}

		public static bool HasValue<T>(this Optional<T> optional) where T: SqlToken {
			return (optional != null) && (optional.Value != null);
		}
	}
}