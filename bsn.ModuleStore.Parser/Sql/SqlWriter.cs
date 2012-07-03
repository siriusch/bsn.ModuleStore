// bsn ModuleStore database versioning
// -----------------------------------
// 
// Copyright 2010 by Arsène von Wyss - avw@gmx.ch
// 
// Development has been supported by Sirius Technologies AG, Basel
// 
// Source:
// 
// https://bsn-modulestore.googlecode.com/hg/
// 
// License:
// 
// The library is distributed under the GNU Lesser General Public License:
// http://www.gnu.org/licenses/lgpl.html
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

using bsn.ModuleStore.Sql.Script;

namespace bsn.ModuleStore.Sql {
	public class SqlWriter {
		private static readonly HashSet<string> azureUnsupportedIndexOption = new HashSet<string> {
				"PAD_INDEX",
				"ALLOW_ROW_LOCKS",
				"ALLOW_PAGE_LOCKS"
		};

		private readonly DatabaseEngine engine;
		private readonly SqlWriterMode mode;
		private readonly TextWriter writer;
		private string indentation = "    ";
		private int indentationLevel;

		public SqlWriter(TextWriter writer, DatabaseEngine engine): this(writer, engine, SqlWriterMode.Normal) {}

		public SqlWriter(TextWriter writer, DatabaseEngine engine, SqlWriterMode mode) {
			if (writer == null) {
				throw new ArgumentNullException("writer");
			}
			this.writer = writer;
			this.engine = engine;
			this.mode = mode;
		}

		public DatabaseEngine Engine {
			get {
				return engine;
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

		public SqlWriterMode Mode {
			get {
				return mode;
			}
		}

		public string NewLine {
			get {
				StringBuilder result = new StringBuilder(Environment.NewLine);
				if (!string.IsNullOrEmpty(indentation)) {
					for (int i = 0; i < indentationLevel; i++) {
						result.Append(indentation);
					}
				}
				return result.ToString();
			}
		}

		public void DecreaseIndent() {
			indentationLevel--;
			Debug.Assert(indentationLevel >= 0);
		}

		public void IncreaseIndent() {
			indentationLevel++;
		}

		public bool IsAtLeast(DatabaseEngine engine) {
			return (this.engine == DatabaseEngine.Unknown) || (this.engine >= engine);
		}

		public void Write(char data) {
			writer.Write(data);
		}

		public void Write(string data) {
			if (!string.IsNullOrEmpty(data)) {
				writer.Write(data);
			}
		}

		public void WriteComment(string comment) {
			if (mode == SqlWriterMode.Normal) {
				WriteLine(comment);
			}
		}

		public void WriteDelimitedIdentifier(string value) {
			if (!string.IsNullOrEmpty(value)) {
				if (value.IndexOf('[') >= 0) {
					writer.Write('"');
					writer.Write(value.Replace(@"""", @""""""));
					writer.Write('"');
				} else {
					writer.Write('[');
					writer.Write(value);
					writer.Write(']');
				}
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

		public void WriteEnum(TableCheck tableCheck, WhitespacePadding padding) {
			if (tableCheck != TableCheck.Unspecified) {
				PaddingBefore(padding);
				Write("WITH ");
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

		public void WriteLine(string text) {
			if (!string.IsNullOrEmpty(text)) {
				Write(text);
			}
			Write(NewLine);
		}

		public void WriteLine() {
			WriteLine(string.Empty);
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
				} else {
					CommentContainerToken comments = value as CommentContainerToken;
					if ((comments != null) && (comments.Comments.Count > 0)) {
						PaddingBefore(padding);
						comments.WriteCommentsTo(this);
					}
				}
			}
		}

		public void WriteScriptSequence<T>(IEnumerable<T> sequence, WhitespacePadding itemPadding, string itemSeparator) where T: SqlScriptableToken {
			if (sequence != null) {
				IEnumerator<T> enumerator = sequence.Where(x => x != null).GetEnumerator();
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

		internal void WriteIndexOptions(IEnumerable<IndexOption> indexOptions, WhitespacePadding itemPadding) {
			if (indexOptions.Any(o => !((engine == DatabaseEngine.SqlAzure) && azureUnsupportedIndexOption.Contains(o.Key.Value)))) {
				PaddingBefore(itemPadding);
				Write("WITH (");
				WriteScriptSequence(indexOptions, WhitespacePadding.None, ", ");
				Write(')');
				PaddingAfter(itemPadding);
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
	}
}
