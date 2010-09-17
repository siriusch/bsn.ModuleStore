using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;

using bsn.ModuleStore.Sql.Script;

namespace bsn.ModuleStore.Sql {
	public class SqlWriter {
		private readonly TextWriter writer;
		private readonly bool emitComments;
		private string indentation = "    ";
		private int indentationLevel;

		public SqlWriter(TextWriter writer): this(writer, true) {}

		public SqlWriter(TextWriter writer, bool emitComments) {
			if (writer == null) {
				throw new ArgumentNullException("writer");
			}
			this.writer = writer;
			this.emitComments = emitComments;
		}

		public void DecreaseIndent() {
			indentationLevel--;
			Debug.Assert(indentationLevel >= 0);
		}

		public void IncreaseIndent() {
			indentationLevel++;
		}

		public void Write(char data) {
			writer.Write(data);
		}

		public void Write(string data) {
			if (!string.IsNullOrEmpty(data)) {
				writer.Write(data);
			}
		}

		public void WriteCommonTableExpressions(ICollection<CommonTableExpression> expressions) {
			if (expressions.Count > 0) {
				Write("WITH ");
				WriteScriptSequence(expressions, WhitespacePadding.NewlineAfter, ",");
			}
		}

		public void WriteDuplicateRestriction(bool? distinct, WhitespacePadding padding) {
			if (distinct.HasValue) {
				PaddingBefore(padding);
				Write(distinct.Value ? "DISTINCT" : "ALL");
				PaddingAfter(padding);
			}
		}

		public void WriteEnum(Clustered clustered, WhitespacePadding padding) {
			if (clustered != Clustered.Unspecified) {
				PaddingBefore(padding);
				switch (clustered) {
				case Clustered.Clustered:
					Write("CLUSTERED");
					break;
				case Clustered.Nonclustered:
					Write("NONCLUSTERED");
					break;
				}
				PaddingAfter(padding);
			}
		}

		public void WriteEnum(IndexFor indexFor, WhitespacePadding padding) {
			if (indexFor != IndexFor.None) {
				PaddingBefore(padding);
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
				PaddingAfter(padding);
			}
		}

		public void WriteEnum(ForXmlKind forXml, WhitespacePadding padding) {
			if (forXml != ForXmlKind.None) {
				PaddingBefore(padding);
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
				PaddingAfter(padding);
			}
		}

		public void WriteEnum(SortOrder order, WhitespacePadding padding) {
			if (order != SortOrder.Unspecified) {
				PaddingBefore(padding);
				switch (order) {
				case SortOrder.Ascending:
					Write("ASC");
					break;
				case SortOrder.Descending:
					Write("DESC");
					break;
				}
				PaddingAfter(padding);
			}
		}

		public void WriteEnum(FulltextChangeTracking changeTracking, WhitespacePadding padding) {
			if (changeTracking != FulltextChangeTracking.Unspecified) {
				PaddingBefore(padding);
				Write("WITH CHANGE TRACKING ");
				if (changeTracking == FulltextChangeTracking.OffNoPopulation) {
					Write("OFF, NO POPULATION");
				} else {
					Write(changeTracking.ToString().ToUpperInvariant());
				}
				PaddingAfter(padding);
			}
		}

		public void WriteEnum(TableCheck tableCheck, WhitespacePadding padding) {
			if (tableCheck != TableCheck.Unspecified) {
				PaddingBefore(padding);
				switch (tableCheck) {
				case TableCheck.Check:
					Write("CHECK");
					break;
				case TableCheck.Nocheck:
					Write("NOCHECK");
					break;
				}
				PaddingAfter(padding);
			}
		}

		public void WriteEnum(FunctionOption functionOption, WhitespacePadding padding) {
			if (functionOption != FunctionOption.None) {
				PaddingBefore(padding);
				Write("WITH ");
				switch (functionOption) {
				case FunctionOption.CalledOnNullInput:
					Write("CALLED ON NULL INPUT");
					break;
				case FunctionOption.ReturnsNullOnNullInput:
					Write("RETURNS NULL ON NULL INPUT");
					break;
				}
				PaddingAfter(padding);
			}
		}

		public void WriteEnum(TriggerType triggerType, WhitespacePadding padding) {
			if (triggerType != TriggerType.None) {
				PaddingBefore(padding);
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
				PaddingAfter(padding);
			}
		}

		public void WriteEnum(DmlOperation operation, WhitespacePadding padding) {
			if (operation != DmlOperation.None) {
				PaddingBefore(padding);
				Write(operation.ToString().ToUpperInvariant());
				PaddingAfter(padding);
			}
		}

		public void WriteEnum(DdlOperation operation, WhitespacePadding padding) {
			if (operation != DdlOperation.None) {
				PaddingBefore(padding);
				Write(operation.ToString().ToUpperInvariant());
				PaddingAfter(padding);
			}
		}

		public void WriteIndexOptions(IEnumerable<IndexOption> indexOptions) {
			using (IEnumerator<IndexOption> enumerator = indexOptions.GetEnumerator()) {
				if (enumerator.MoveNext()) {
					Write(" WITH (");
					WriteScriptSequence(indexOptions, WhitespacePadding.None, ", ");
					Write(')');
				}
			}
		}

		public string Indentation {
			get {
				return indentation;
			}
			set {
				indentation = value ?? string.Empty;
			}
		}

		public void WriteLine(string text) {
			if (!string.IsNullOrEmpty(text)) {
				Write(text);
			}
			Write(Environment.NewLine);
			if (!string.IsNullOrEmpty(indentation)) {
				for (int i = 0; i < indentationLevel; i++) {
					Write(indentation);
				}
			}
		}

		public void WriteLine() {
			WriteLine(string.Empty);
		}

		public void WriteNotForReplication(bool notForReplication, WhitespacePadding padding) {
			if (notForReplication) {
				PaddingBefore(padding);
				Write("NOT FOR REPLICATION");
				PaddingAfter(padding);
			}
		}

		public void WriteScript<T>(T value, WhitespacePadding padding) where T: SqlScriptableToken {
			WriteScript(value, padding, null, null);
		}

		public void WriteScript<T>(T value, WhitespacePadding padding, string prefix, string suffix) where T: SqlScriptableToken {
			if (value != null) {
				IOptional optional = value as IOptional;
				if ((optional == null) || (optional.HasValue)) {
					PaddingBefore(padding);
					Write(prefix);
					value.WriteTo(this);
					Write(suffix);
					PaddingAfter(padding);
				}
			}
		}

		public void WriteScriptSequence<T>(IEnumerable<T> sequence, WhitespacePadding itemPadding, string itemSeparator) where T: SqlScriptableToken {
			if (sequence != null) {
				IEnumerator<T> enumerator = sequence.GetEnumerator();
				if (enumerator.MoveNext()) {
					PaddingBefore(itemPadding);
					WriteScript(enumerator.Current, WhitespacePadding.None);
					while (enumerator.MoveNext()) {
						Write(itemSeparator);
						PaddingAfter(itemPadding);
						PaddingBefore(itemPadding);
						WriteScript(enumerator.Current, WhitespacePadding.None);
					}
					PaddingAfter(itemPadding);
				}
			}
		}

		public void WriteToggle(bool? toggle, WhitespacePadding padding) {
			if (toggle.HasValue) {
				PaddingBefore(padding);
				Write(toggle.Value ? "ON" : "OFF");
				PaddingAfter(padding);
			}
		}

		private void PaddingAfter(WhitespacePadding padding) {
			switch (padding) {
			case WhitespacePadding.NewlineAfter:
				WriteLine();
				break;
			case WhitespacePadding.SpaceAfter:
				Write(' ');
				break;
			}
		}

		private void PaddingBefore(WhitespacePadding padding) {
			switch (padding) {
			case WhitespacePadding.NewlineBefore:
				WriteLine();
				break;
			case WhitespacePadding.SpaceBefore:
				Write(' ');
				break;
			}
		}

		public void WriteComment(string comment) {
			if (emitComments) {
				WriteLine(comment);
			}
		}
	}
}