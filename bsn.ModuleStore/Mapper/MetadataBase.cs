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
using System.Globalization;
using System.Xml.Linq;

namespace bsn.ModuleStore.Mapper {
	public abstract class MetadataBase<T> {
		public static implicit operator T(MetadataBase<T> metadataValue) {
			return metadataValue.GetValue();
		}

		private readonly XName elementName;
		private readonly Func<XDocument> metadata;

		protected MetadataBase(Func<XDocument> metadata, XName elementName) {
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

		public T GetValue(XDocument metadata, CultureInfo culture) {
			return ToValueInternal(GetValueString(metadata, culture));
		}

		public T GetValue(CultureInfo culture) {
			return GetValue(GetMetadata(), culture);
		}

		public T GetValue(XDocument metadata) {
			return GetValue(metadata, null);
		}

		public T GetValue() {
			return GetValue(GetMetadata(), CultureInfo.CurrentUICulture);
		}

		public void Set(XDocument metadata, CultureInfo culture, T value) {
			if (metadata == null) {
				throw new ArgumentNullException("metadata");
			}
			metadata.Set(elementName, culture, ToStringInternal(value));
		}

		public XDocument Set(CultureInfo culture, T value) {
			XDocument result = GetMetadata().Clone();
			Set(result, culture, value);
			return result;
		}

		public XDocument Set(T value) {
			return Set((CultureInfo)null, value);
		}

		public void Set(XDocument metadata, T value) {
			Set(metadata, null, value);
		}

		public override sealed string ToString() {
			return GetValueString(GetMetadata(), CultureInfo.CurrentUICulture);
		}

		protected XDocument GetMetadata() {
			if (metadata == null) {
				throw new InvalidOperationException("The given metadata object was not associated to a specific metadata");
			}
			return metadata();
		}

		protected abstract string ToStringInternal(T value);

		protected abstract T ToValueInternal(string value);

		private string GetValueString(XDocument metadata, CultureInfo culture) {
			return metadata.Get(elementName, culture);
		}
	}
}
