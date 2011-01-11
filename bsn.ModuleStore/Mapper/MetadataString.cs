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
//  
using System;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace bsn.ModuleStore.Mapper {
	public class MetadataString {
		public static implicit operator String(MetadataString metadataString) {
			return metadataString.ToString();
		}

		private readonly XName elementName;
		private readonly Func<XDocument> metadata;

		public MetadataString(Func<XDocument> metadata, XName elementName) {
			if (metadata == null) {
				throw new ArgumentNullException("metadata");
			}
			if (elementName == null) {
				throw new ArgumentNullException("elementName");
			}
			this.metadata = metadata;
			this.elementName = elementName;
		}

		protected XName ElementName {
			get {
				return elementName;
			}
		}

		protected XDocument Metadata {
			get {
				return metadata();
			}
		}

		public void Set(XDocument metadata, CultureInfo culture, string value) {
			if (metadata == null) {
				throw new ArgumentNullException("metadata");
			}
			metadata.Set(elementName, culture, value);
		}

		public XDocument Set(CultureInfo culture, string value) {
			XDocument result = Metadata.Clone();
			Set(result, culture, value);
			return result;
		}

		public XDocument Set(string value) {
			return Set((CultureInfo)null, value);
		}

		public void Set(XDocument metadata, string value) {
			Set(metadata, null, value);
		}

		public virtual string ToString(XDocument metadata, CultureInfo culture) {
			return metadata.Get(elementName, culture);
		}

		public string ToString(CultureInfo culture) {
			return ToString(Metadata, culture);
		}

		public string ToString(XDocument metadata) {
			return ToString(metadata, null);
		}

		public sealed override string ToString() {
			return ToString(Metadata, CultureInfo.CurrentUICulture);
		}
	}
}