﻿// bsn ModuleStore database versioning
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
using System.Xml;

namespace bsn.ModuleStore.Mapper.Serialization {
	internal class XmlElementMemberConverter: XmlReaderMemberConverterBase {
		public XmlElementMemberConverter(Type type, bool isIdentity, string columnName, int memberIndex): base(type, isIdentity, columnName, memberIndex) {}

		protected override object GetXmlObject(IDeserializerContext context, XmlReader reader) {
			if (HasContent(reader)) {
				context.XmlDocument.Load(reader);
				var result = context.XmlDocument.DocumentElement;
				context.XmlDocument.RemoveAll();
				return result;
			}
			return null;
		}
	}
}
