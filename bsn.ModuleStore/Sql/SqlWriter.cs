using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

using bsn.ModuleStore.Sql.Script;

namespace bsn.ModuleStore.Sql {
	public class SqlWriter {
		private readonly TextWriter writer;
		private string indentation;
		private int indentationLevel;
		private SchemaName schemaName;

		public SqlWriter(TextWriter writer) {
			if (writer == null) {
				throw new ArgumentNullException("writer");
			}
			this.writer = writer;
		}

		public void Write(char data) {
			writer.Write(data);
		}

		public void Write(string data) {
			writer.Write(data);
		}

		public void WriteCommonTableExpressions(ICollection<CommonTableExpression> expressions) {
			if (expressions.Count > 0) {
				Write("WITH ");
				WriteSequence(expressions, null, ",", Environment.NewLine);
			}
		}

		public void WriteDuplicateRestriction(bool? distinct, string prefix, string suffix) {
			if (distinct.HasValue) {
				WriteString(prefix);
				Write(distinct.Value ? "DISTINCT" : "ALL");
				WriteString(suffix);
			}
		}

		public void WriteIndexOptions(ICollection<IndexOption> indexOptions) {
			if (indexOptions.Count > 0) {
				Write(" WITH (");
				WriteSequence(indexOptions, null, ", ", null);
				Write(')');
			}
		}

		public void WriteLine(string text) {
			Write(text);
			Write(Environment.NewLine);
		}

		public void WriteLine() {
			WriteLine(string.Empty);
		}

		public void WriteNotForReplication(bool notForReplication, string prefix, string suffix) {
			if (notForReplication) {
				WriteString(prefix);
				Write("NOT FOR REPLICATION");
				WriteString(suffix);
			}
		}

		public void WritePercent(bool percent, string prefix, string suffix) {
			if (percent) {
				WriteString(prefix);
				Write("PERCENTN");
				WriteString(suffix);
			}
		}

		public void WritePrimary(bool primary, string prefix, string suffix) {
			if (primary) {
				WriteString(prefix);
				Write("PRIMARY");
				WriteString(suffix);
			}
		}

		public void WriteScript<T>(T value) where T: SqlToken, IScriptable {
			WriteScript(value, null, null);
		}

		public void WriteScript<T>(T value, string prefix, string suffix) where T: SqlToken, IScriptable {
			if (value != null) {
				IOptional optional = value as IOptional;
				if ((optional == null) || (optional.HasValue)) {
					WriteString(prefix);
					value.WriteTo(this);
					WriteString(suffix);
				}
			}
		}

		public void WriteSequence<T>(IEnumerable<T> sequence, string itemPrefix, string itemSeparator, string itemSuffix) where T: SqlToken, IScriptable {
			if (sequence != null) {
				IEnumerator<T> enumerator = sequence.GetEnumerator();
				if (enumerator.MoveNext()) {
					WriteString(itemPrefix);
					WriteScript(enumerator.Current);
					while (enumerator.MoveNext()) {
						WriteString(itemSeparator);
						WriteString(itemSuffix);
						WriteString(itemPrefix);
						WriteScript(enumerator.Current);
					}
					WriteString(itemSuffix);
				}
			}
		}

		public void WriteToggle(bool? toggle, string prefix, string suffix) {
			if (toggle.HasValue) {
				WriteString(prefix);
				Write(toggle.Value ? "ON" : "OFF");
				WriteString(suffix);
			}
		}

		public void WriteValue(Clustered clustered, string prefix, string suffix) {
			if (clustered != Clustered.Unspecified) {
				WriteString(prefix);
				switch (clustered) {
				case Clustered.Clustered:
					Write("CLUSTERED");
					break;
				case Clustered.Nonclustered:
					Write("NONCLUSTERED");
					break;
				}
				WriteString(suffix);
			}
		}

		public void WriteValue(IndexFor indexFor, string prefix, string suffix) {
			if (indexFor != IndexFor.None) {
				WriteString(prefix);
				switch (indexFor) {
				case IndexFor.Value:
					Write("FOR VALUE");
					break;
				case IndexFor.Path:
					Write("FOR PATH");
					break;
				case IndexFor.Property:
					Write("FOR PROPERTY");
					break;
				}
				WriteString(suffix);
			}
		}

		public void WriteValue(ForXmlKind forXml, string prefix, string suffix) {
			if (forXml != ForXmlKind.None) {
				WriteString(prefix);
				Write("FOR XML ");
				switch (forXml) {
				case ForXmlKind.Auto:
					Write("AUTO");
					break;
				case ForXmlKind.Path:
					Write("PATH");
					break;
				case ForXmlKind.Raw:
					Write("RAW");
					break;
				case ForXmlKind.Explicit:
					Write("EXPLICIT");
					break;
				}
				WriteString(suffix);
			}
		}

		public void WriteValue(SortOrder order, string prefix, string suffix) {
			if (order != SortOrder.Unspecified) {
				WriteString(prefix);
				switch (order) {
				case SortOrder.Ascending:
					Write("ASC");
					break;
				case SortOrder.Descending:
					Write("DESC");
					break;
				}
				WriteString(suffix);
			}
		}

		public void WriteValue(FulltextChangeTracking changeTracking, string prefix, string suffix) {
			if (changeTracking != FulltextChangeTracking.Unspecified) {
				WriteString(prefix);
				Write("WITH CHANGE TRACKING ");
				if (changeTracking == FulltextChangeTracking.OffNoPopulation) {
					Write("OFF, NO POPULATION");
				} else {
					Write(changeTracking.ToString().ToUpperInvariant());
				}
				WriteString(suffix);
			}
		}

		public void WriteValue(TableCheck tableCheck, string prefix, string suffix) {
			if (tableCheck != TableCheck.Unspecified) {
				WriteString(prefix);
				switch (tableCheck) {
				case TableCheck.Check:
					Write("CHECK");
					break;
				case TableCheck.Nocheck:
					Write("NOCHECK");
					break;
				}
				WriteString(suffix);
			}
		}

		public void WriteValue(FunctionOption functionOption, string prefix, string suffix) {
			if (functionOption != FunctionOption.None) {
				WriteString(prefix);
				Write("WITH ");
				switch (functionOption) {
				case FunctionOption.CalledOnNullInput:
					Write("CALLED ON NULL INPUT");
					break;
				case FunctionOption.ReturnsNullOnNullInput:
					Write("RETURNS NULL ON NULL INPUT");
					break;
				}
				WriteString(suffix);
			}
		}

		public void WriteValue(TriggerType triggerType, string prefix, string suffix) {
			if (triggerType != TriggerType.None) {
				WriteString(prefix);
				switch (triggerType) {
				case TriggerType.After:
					Write("AFTER");
					break;
				case TriggerType.InsteadOf:
					Write("INSTEAD OF");
					break;
				case TriggerType.For:
					Write("FOR");
					break;
				}
				WriteString(suffix);
			}
		}

		public void WriteValue(DmlOperation operation, string prefix, string suffix) {
			if (operation != DmlOperation.None) {
				WriteString(prefix);
				Write(operation.ToString().ToUpperInvariant());
				WriteString(suffix);
			}
		}

		public void WriteValue(DdlOperation operation, string prefix, string suffix) {
			if (operation != DdlOperation.None) {
				WriteString(prefix);
				Write(operation.ToString().ToUpperInvariant());
				WriteString(suffix);
			}
		}

		public void WriteWithCheckOption(bool withCheckOption, string prefix, string suffix) {
			if (withCheckOption) {
				WriteString(prefix);
				Write("WITH CHECK OPTION");
				WriteString(suffix);
			}
		}

		public void WriteWithRecompile(bool withCheckOption, string prefix, string suffix) {
			if (withCheckOption) {
				WriteString(prefix);
				Write("WITH RECOMPILE");
				WriteString(suffix);
			}
		}

		public void WriteWithTies(bool withTies, string prefix, string suffix) {
			if (withTies) {
				WriteString(prefix);
				Write("WITH TIES");
				WriteString(suffix);
			}
		}

		public void WriteWithViewMetadata(bool withViewMetadata, string prefix, string suffix) {
			if (withViewMetadata) {
				WriteString(prefix);
				Write("WITH VIEW_METADATA");
				WriteString(suffix);
			}
		}

		private void WriteString(string value) {
			if (!string.IsNullOrEmpty(value)) {
				Write(value);
			}
		}
	}
}
