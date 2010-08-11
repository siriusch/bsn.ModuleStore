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

		public static void WriteSequence<T>(this TextWriter writer, IEnumerable<T> sequence, string itemPrefix, string itemSeparator, string itemSuffix) where T: SqlToken, IScriptable {
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

		public static void WriteIndexOptions(this TextWriter writer, ICollection<IndexOption> indexOptions) {
			if (indexOptions.Count > 0) {
				writer.Write(" WITH (");
				writer.WriteSequence(indexOptions, null, ", ", null);
				writer.Write(')');
			}
		}

		public static void WriteScript<T>(this TextWriter writer, T value) where T: SqlToken, IScriptable {
			WriteScript(writer, value, null, null);
		}

		public static void WriteScript<T>(this TextWriter writer, T value, string prefix, string suffix) where T: SqlToken, IScriptable {
			if (value != null) {
				IOptional optional = value as IOptional;
				if ((optional == null) || (optional.HasValue)) {
					WriteString(writer, prefix);
					value.WriteTo(writer);
					WriteString(writer, suffix);
				}
			}
		}

		private static void WriteString(TextWriter writer, string value) {
			if (!string.IsNullOrEmpty(value)) {
				writer.Write(value);
			}
		}

		public static void WriteValue(this TextWriter writer, Clustered clustered, string prefix, string suffix) {
			if (clustered != Clustered.Unspecified) {
				WriteString(writer, prefix);
				switch (clustered) {
				case Clustered.Clustered:
					writer.Write("CLUSTERED");
					break;
				case Clustered.Nonclustered:
					writer.Write("NONCLUSTERED");
					break;
				}
				WriteString(writer, suffix);
			}
		}

		public static void WriteValue(this TextWriter writer, IndexFor indexFor, string prefix, string suffix) {
			if (indexFor != IndexFor.None) {
				WriteString(writer, prefix);
				switch (indexFor) {
				case IndexFor.Value:
					writer.Write("FOR VALUE");
					break;
				case IndexFor.Path:
					writer.Write("FOR PATH");
					break;
				case IndexFor.Property:
					writer.Write("FOR PROPERTY");
					break;
				}
				WriteString(writer, suffix);
			}
		}

		public static void WriteValue(this TextWriter writer, ForXmlKind forXml, string prefix, string suffix) {
			if (forXml != ForXmlKind.None) {
				WriteString(writer, prefix);
				writer.Write("FOR XML ");
				switch (forXml) {
				case ForXmlKind.Auto:
					writer.Write("AUTO");
					break;
				case ForXmlKind.Path:
					writer.Write("PATH");
					break;
				case ForXmlKind.Raw:
					writer.Write("RAW");
					break;
				case ForXmlKind.Explicit:
					writer.Write("EXPLICIT");
					break;
				}
				WriteString(writer, suffix);
			}
		}

		public static void WriteValue(this TextWriter writer, SortOrder order, string prefix, string suffix) {
			if (order != SortOrder.Unspecified) {
				WriteString(writer, prefix);
				switch (order) {
				case SortOrder.Ascending:
					writer.Write("ASC");
					break;
				case SortOrder.Descending:
					writer.Write("DESC");
					break;
				}
				WriteString(writer, suffix);
			}
		}

		public static void WriteValue(this TextWriter writer, FulltextChangeTracking changeTracking, string prefix, string suffix) {
			if (changeTracking != FulltextChangeTracking.Unspecified) {
				WriteString(writer, prefix);
				writer.Write("WITH CHANGE TRACKING ");
				if (changeTracking == FulltextChangeTracking.OffNoPopulation) {
					writer.Write("OFF, NO POPULATION");
				} else {
					writer.Write(changeTracking.ToString().ToUpperInvariant());
				}
				WriteString(writer, suffix);
			}
		}

		public static void WriteValue(this TextWriter writer, TableCheck tableCheck, string prefix, string suffix) {
			if (tableCheck != TableCheck.Unspecified) {
				WriteString(writer, prefix);
				switch (tableCheck) {
				case TableCheck.Check:
					writer.Write("CHECK");
					break;
				case TableCheck.Nocheck:
					writer.Write("NOCHECK");
					break;
				}
				WriteString(writer, suffix);
			}
		}

		public static void WriteValue(this TextWriter writer, FunctionOption functionOption, string prefix, string suffix) {
			if (functionOption != FunctionOption.None) {
				WriteString(writer, prefix);
				writer.Write("WITH ");
				switch (functionOption) {
				case FunctionOption.CalledOnNullInput:
					writer.Write("CALLED ON NULL INPUT");
					break;
				case FunctionOption.ReturnsNullOnNullInput:
					writer.Write("RETURNS NULL ON NULL INPUT");
					break;
				}
				WriteString(writer, suffix);
			}
		}

		public static void WriteValue(this TextWriter writer, TriggerType triggerType, string prefix, string suffix) {
			if (triggerType != TriggerType.None) {
				WriteString(writer, prefix);
				switch (triggerType) {
				case TriggerType.After:
					writer.Write("AFTER");
					break;
				case TriggerType.InsteadOf:
					writer.Write("INSTEAD OF");
					break;
				case TriggerType.For:
					writer.Write("FOR");
					break;
				}
				WriteString(writer, suffix);
			}
		}

		public static void WriteValue(this TextWriter writer, DmlOperation operation, string prefix, string suffix) {
			if (operation != DmlOperation.None) {
				WriteString(writer, prefix);
				writer.Write(operation.ToString().ToUpperInvariant());
				WriteString(writer, suffix);
			}
		}

		public static void WriteToggle(this TextWriter writer, bool? toggle, string prefix, string suffix) {
			if (toggle.HasValue) {
				WriteString(writer, prefix);
				writer.Write(toggle.Value ? "ON" : "OFF");
				WriteString(writer, suffix);
			}
		}

		public static void WriteDuplicateRestriction(this TextWriter writer, bool? distinct, string prefix, string suffix) {
			if (distinct.HasValue) {
				WriteString(writer, prefix);
				writer.Write(distinct.Value ? "DISTINCT" : "ALL");
				WriteString(writer, suffix);
			}
		}

		public static void WritePrimary(this TextWriter writer, bool primary, string prefix, string suffix) {
			if (primary) {
				WriteString(writer, prefix);
				writer.Write("PRIMARY");
				WriteString(writer, suffix);
			}
		}

		public static void WriteNotForReplication(this TextWriter writer, bool notForReplication, string prefix, string suffix) {
			if (notForReplication) {
				WriteString(writer, prefix);
				writer.Write("NOT FOR REPLICATION");
				WriteString(writer, suffix);
			}
		}

		public static void WriteWithViewMetadata(this TextWriter writer, bool withViewMetadata, string prefix, string suffix) {
			if (withViewMetadata) {
				WriteString(writer, prefix);
				writer.Write("WITH VIEW_METADATA");
				WriteString(writer, suffix);
			}
		}

		public static void WriteWithCheckOption(this TextWriter writer, bool withCheckOption, string prefix, string suffix) {
			if (withCheckOption) {
				WriteString(writer, prefix);
				writer.Write("WITH CHECK OPTION");
				WriteString(writer, suffix);
			}
		}

		public static void WriteWithRecompile(this TextWriter writer, bool withCheckOption, string prefix, string suffix) {
			if (withCheckOption) {
				WriteString(writer, prefix);
				writer.Write("WITH RECOMPILE");
				WriteString(writer, suffix);
			}
		}

		public static void WritePercent(this TextWriter writer, bool percent, string prefix, string suffix) {
			if (percent) {
				WriteString(writer, prefix);
				writer.Write("PERCENTN");
				WriteString(writer, suffix);
			}
		}

		public static void WriteWithTies(this TextWriter writer, bool withTies, string prefix, string suffix) {
			if (withTies) {
				WriteString(writer, prefix);
				writer.Write("WITH TIES");
				WriteString(writer, suffix);
			}
		}

		public static void WriteCommonTableExpressions(this TextWriter writer, ICollection<CommonTableExpression> expressions) {
			if (expressions.Count > 0) {
				writer.Write("WITH ");
				writer.WriteSequence(expressions, null, ",", Environment.NewLine);
			}
		}
	}
}