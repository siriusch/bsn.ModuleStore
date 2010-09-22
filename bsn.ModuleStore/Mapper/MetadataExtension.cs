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
			} else{
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
