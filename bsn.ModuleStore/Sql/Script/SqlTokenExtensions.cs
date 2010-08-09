using System;
using System.Collections.Generic;
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

		public static void WriteSequence<T>(this TextWriter writer, IEnumerable<T> sequence, string itemPrefix, string itemSeparator, string itemSuffix) where T: SqlToken {
			if (sequence != null) {
				IEnumerator<T> enumerator = sequence.GetEnumerator();
				if (enumerator.MoveNext()) {
					WriteString(writer, itemPrefix);
					WriteScript(writer, enumerator.Current);
					while (enumerator.MoveNext()) {
						WriteString(writer, itemSeparator);
						WriteString(writer, itemSuffix);
						WriteString(writer, itemPrefix);
						WriteScript(writer, enumerator.Current);
					}
					WriteString(writer, itemSuffix);
				}
			}
		}

		public static void WriteScript<T>(this TextWriter writer, T value) where T: SqlToken {
			if (value != null) {
				value.WriteTo(writer);
			}
		}

		private static void WriteString(TextWriter writer, string value) {
			if (!string.IsNullOrEmpty(value)) {
				writer.Write(value);
			}
		}
	}
}