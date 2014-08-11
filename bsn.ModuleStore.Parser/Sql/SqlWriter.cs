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
using System.IO;
using System.Linq;

using bsn.GoldParser.Text;
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
		private readonly RichTextWriter writer;

		public SqlWriter(TextWriter writer, DatabaseEngine engine): this(writer, engine, SqlWriterMode.Normal) {}

		public SqlWriter(TextWriter writer, DatabaseEngine engine, SqlWriterMode mode) {
			if (writer == null) {
				throw new ArgumentNullException("writer");
			}
			this.writer = writer as RichTextWriter ?? RichTextWriter.Wrap(writer);
			this.engine = engine;
			this.mode = mode;
			Indentation = "    ";
		}

		public DatabaseEngine Engine {
			get {
				return engine;
			}
		}

		public string Indentation {
			get {
				return writer.IndentChars;
			}
			set {
				writer.IndentChars = value ?? string.Empty;
			}
		}

		public SqlWriterMode Mode {
			get {
				return mode;
			}
		}

		public IDisposable Indent() {
			return writer.Indent();
		}

		public bool IsAtLeast(DatabaseEngine engine) {
			return (this.engine == DatabaseEngine.Unknown) || (this.engine >= engine);
		}

		public void Write(char data) {
			if (!char.IsWhiteSpace(data)) {
				writer.SetStyle(SqlTextKind.Normal);
			}
			writer.Write(data);
		}

		public void Write(string data) {
			writer.SetStyle(SqlTextKind.Normal);
			writer.Write(data);
		}

		public void WriteComment(string comment) {
			if (mode == SqlWriterMode.Normal) {
				writer.SetStyle(SqlTextKind.Comment);
				writer.WriteLine(comment);
			}
		}

		public void WriteDelimitedIdentifier(string value) {
			if (!string.IsNullOrEmpty(value)) {
				writer.SetStyle(SqlTextKind.Identifier);
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
				WriteKeyword(distinct.Value ? "DISTINCT" : "ALL");
				PaddingAfter(padding);
			}
		}

		public void WriteEnum(Clustered clustered, WhitespacePadding padding) {
			if (clustered != Clustered.Unspecified) {
				PaddingBefore(padding);
				switch (clustered) {
				case Clustered.Clustered:
					WriteKeyword("CLUSTERED");
					break;
				case Clustered.Nonclustered:
					WriteKeyword("NONCLUSTERED");
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
					WriteKeyword("ASC");
					break;
				case SortOrder.Descending:
					WriteKeyword("DESC");
					break;
				}
				PaddingAfter(padding);
			}
		}

		public void WriteEnum(TableCheck tableCheck, WhitespacePadding padding) {
			if (tableCheck != TableCheck.Unspecified) {
				PaddingBefore(padding);
				WriteKeyword("WITH ");
				switch (tableCheck) {
				case TableCheck.Check:
					WriteKeyword("CHECK");
					break;
				case TableCheck.Nocheck:
					WriteKeyword("NOCHECK");
					break;
				}
				PaddingAfter(padding);
			}
		}

		public void WriteEnum(DmlOperation operation, WhitespacePadding padding) {
			if (operation != DmlOperation.None) {
				PaddingBefore(padding);
				WriteKeyword(operation.ToString().ToUpperInvariant());
				PaddingAfter(padding);
			}
		}

		public void WriteEnum(DdlOperation operation, WhitespacePadding padding) {
			if (operation != DdlOperation.None) {
				PaddingBefore(padding);
				WriteKeyword(operation.ToString().ToUpperInvariant());
				PaddingAfter(padding);
			}
		}

		public void WriteFunction(string value) {
			writer.SetStyle(SqlTextKind.Function);
			writer.Write(value);
		}

		public void WriteIdentifier(string value) {
			writer.SetStyle(SqlTextKind.Identifier);
			writer.Write(value);
		}

		public void WriteKeyword(string keyword) {
			writer.SetStyle(SqlTextKind.Keyword);
			writer.Write(keyword);
		}

		public void WriteLine(string text) {
			writer.SetStyle(SqlTextKind.Normal);
			writer.WriteLine(text);
		}

		public void WriteLine() {
			writer.WriteLine();
		}

		public void WriteLiteral(string value) {
			writer.SetStyle(SqlTextKind.Literal);
			writer.Write(value);
		}

		public void WriteLiteral(char value) {
			writer.SetStyle(SqlTextKind.Literal);
			writer.Write(value);
		}

		public void WriteOperator(string value) {
			writer.SetStyle(SqlTextKind.Operator);
			writer.Write(value);
		}

		public void WriteScript<T>(T value, WhitespacePadding padding) where T: SqlScriptableToken {
			WriteScript(value, padding, null, null);
		}

		public void WriteScript<T>(T value, WhitespacePadding padding, Action<SqlWriter> prefix, Action<SqlWriter> suffix) where T: SqlScriptableToken {
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

		public void WriteScriptSequence<T>(IEnumerable<T> sequence, WhitespacePadding itemPadding, Action<SqlWriter> separator) where T: SqlScriptableToken {
			if (sequence != null) {
				IEnumerator<T> enumerator = sequence.Where(x => x != null).GetEnumerator();
				if (enumerator.MoveNext()) {
					PaddingBefore(itemPadding);
					WriteScript(enumerator.Current, WhitespacePadding.None);
					while (enumerator.MoveNext()) {
						Write(separator);
						PaddingAfter(itemPadding);
						PaddingBefore(itemPadding);
						WriteScript(enumerator.Current, WhitespacePadding.None);
					}
					PaddingAfter(itemPadding);
				}
			}
		}

		public void WriteString(string value) {
			writer.SetStyle(SqlTextKind.String);
			writer.Write(value);
		}

		public void WriteToggle(bool? toggle, WhitespacePadding padding) {
			if (toggle.HasValue) {
				PaddingBefore(padding);
				WriteKeyword(toggle.Value ? "ON" : "OFF");
				PaddingAfter(padding);
			}
		}

		public void WriteType(string value) {
			writer.SetStyle(SqlTextKind.Type);
			writer.Write(value);
		}

		internal void WriteIndexOptions(IEnumerable<IndexOption> indexOptions, WhitespacePadding itemPadding) {
			// ignore any unwanted options; either unsupported by Azure or FILLFACTOR during hashing
			ICollection<IndexOption> indexOptionsToRender = indexOptions.Where(o => !(((engine == DatabaseEngine.SqlAzure) && azureUnsupportedIndexOption.Contains(o.Key.Value)) || ((mode == SqlWriterMode.ForHashing) && o.Key.Value.Equals("FILLFACTOR", StringComparison.OrdinalIgnoreCase)))).ToList();
			if (indexOptionsToRender.Count > 0) {
				PaddingBefore(itemPadding);
				WriteKeyword("WITH ");
				Write('(');
				WriteScriptSequence(indexOptionsToRender, WhitespacePadding.None, w => w.Write(", "));
				Write(')');
				PaddingAfter(itemPadding);
			}
		}

		private void PaddingAfter(WhitespacePadding padding) {
			switch (padding) {
			case WhitespacePadding.NewlineAfter:
				writer.WriteLine();
				break;
			case WhitespacePadding.SpaceAfter:
				writer.Write(' ');
				break;
			}
		}

		private void PaddingBefore(WhitespacePadding padding) {
			switch (padding) {
			case WhitespacePadding.NewlineBefore:
				writer.WriteLine();
				break;
			case WhitespacePadding.SpaceBefore:
				writer.Write(' ');
				break;
			}
		}

		private void Write(Action<SqlWriter> action) {
			if (action != null) {
				action(this);
			}
		}
	}
}
