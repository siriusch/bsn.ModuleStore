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
//  
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;

namespace bsn.ModuleStore.Mapper {
	public static class MetadataExtension {
		public static string Get(this XDocument doc, XName elementName) {
			return Get(doc, elementName, string.Empty);
		}

		public static string Get(this XDocument doc, XName elementName, string @default) {
			if (elementName == null) {
				throw new ArgumentNullException("elementName");
			}
			if (doc != null) {
				XElement root = doc.Root;
				if (root != null) {
					XElement value = root.Element(elementName);
					if (value != null) {
						return value.Value;
					}
				}
			}
			return @default;
		}

		public static IEnumerable<XElement> GetAll(this XDocument doc) {
			if (doc != null) {
				XElement root = doc.Root;
				if (root != null) {
					return root.Elements();
				}
			}
			return new XElement[0];
		}

		public static void Set(this XDocument doc, XName elementName, string value) {
			if (doc == null) {
				throw new ArgumentNullException("doc");
			}
			if (elementName == null) {
				throw new ArgumentNullException("elementName");
			}
			XElement root = doc.GetRoot();
			if (value == null) {
				root.Elements(elementName).Remove();
			} else {
				using (IEnumerator<XElement> enumerator = root.Elements(elementName).GetEnumerator()) {
					if (enumerator.MoveNext()) {
						XElement current = enumerator.Current;
						Debug.Assert(current != null);
						current.RemoveAll();
						current.Value = value;
						do {
							current = enumerator.Current;
							Debug.Assert(current != null);
							current.Remove();
						} while (enumerator.MoveNext());
					} else {
						root.Add(new XElement(elementName, value));
					}
				}
			}
		}

		public static XElement GetRoot(this XDocument doc) {
			if (doc == null) {
				throw new ArgumentNullException("doc");
			}
			XElement root = doc.Root;
			if (root == null) {
				root = new XElement("xml");
				doc.Add(root);
			}
			return root;
		}

		public static void Add(this XDocument doc, XName elementName, string value) {
			if (doc == null) {
				throw new ArgumentNullException("doc");
			}
			if (elementName == null) {
				throw new ArgumentNullException("elementName");
			}
			if (value == null) {
				throw new ArgumentNullException("value");
			}
			doc.GetRoot().Add(new XElement(elementName, value));
		}
	}
}
