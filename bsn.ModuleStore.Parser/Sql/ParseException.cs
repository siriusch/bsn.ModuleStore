using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using bsn.GoldParser.Grammar;
using bsn.GoldParser.Parser;

namespace bsn.ModuleStore.Sql {
	[Serializable]
	public class ParseException: Exception {
		private readonly ParseMessage parseMessage;
		private readonly LineInfo position;
		private string fileName;

		public ParseException(string message, ParseMessage parseMessage, LineInfo position): base(message) {
			this.parseMessage = parseMessage;
			this.position = position;
		}

		public ParseException(string message, ParseMessage parseMessage, LineInfo position, Exception innerException): base(message, innerException) {
			this.position = position;
			this.parseMessage = parseMessage;
		}

		protected ParseException(SerializationInfo info, StreamingContext context): base(info, context) {}

		public ParseMessage ParseMessage {
			get {
				return parseMessage;
			}
		}

		public LineInfo Position {
			get {
				return position;
			}
		}

		public string FileName {
			get {
				return fileName ?? string.Empty;
			}
			internal set {
				fileName = value;
			}
		}

		public override string Message {
			get {
				StringBuilder result = new StringBuilder(base.Message);
				result.AppendLine();
				result.Append(parseMessage);
				result.Append(" @ ");
				if (!string.IsNullOrEmpty(fileName)) {
					result.Append(fileName);
					result.Append(' ');
				}
				result.Append(position);
				return result.ToString();
			}
		}
	}
}