﻿using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class UpdateModeReadOnly: UpdateMode {
		[Rule("<CursorUpdate> ::= READ_ONLY", AllowTruncationForConstructor = true)]
		public UpdateModeReadOnly() {}

		public override UpdateModeKind Kind {
			get {
				return UpdateModeKind.ReadOnly;
			}
		}

		public override void WriteTo(TextWriter writer) {
			writer.Write("READ ONLY");
		}
	}
}