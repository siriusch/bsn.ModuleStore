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
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;

namespace bsn.ModuleStore.Mapper {
	public static class MetadataExtension {
		public static readonly XName XmlLang = XName.Get("lang", "http://www.w3.org/XML/1998/namespace");

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

		public static XDocument Clone(this XDocument doc) {
			if (doc == null) {
				return new XDocument();
			}
			return new XDocument(doc);
		}

		public static string Get(this XDocument doc, XName elementName) {
			return Get(doc, elementName, null, string.Empty);
		}

		public static string Get(this XDocument doc, XName elementName, string @default) {
			return Get(doc, elementName, null, @default);
		}

		public static string Get(this XDocument doc, XName elementName, CultureInfo culture) {
			return Get(doc, elementName, culture, string.Empty);
		}

		public static string Get(this XDocument doc, XName elementName, CultureInfo culture, string @default) {
			XElement element = doc.GetElement(elementName, culture, true);
			if (element != null) {
				return element.Value;
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

		public static bool? GetBoolean(this XDocument doc, XName elementName, bool? @default) {
			string result = Get(doc, elementName, null, null);
			if (result == null) {
				return @default;
			}
			return XmlConvert.ToBoolean(result);
		}

		public static bool? GetBoolean(this XDocument doc, XName elementName) {
			return GetBoolean(doc, elementName, null);
		}

		public static byte? GetByte(this XDocument doc, XName elementName, byte? @default) {
			string result = Get(doc, elementName, null, null);
			if (result == null) {
				return @default;
			}
			return XmlConvert.ToByte(result);
		}

		public static byte? GetByte(this XDocument doc, XName elementName) {
			return GetByte(doc, elementName, null);
		}

		public static char? GetChar(this XDocument doc, XName elementName, char? @default) {
			string result = Get(doc, elementName, null, null);
			if (result == null) {
				return @default;
			}
			return XmlConvert.ToChar(result);
		}

		public static char? GetChar(this XDocument doc, XName elementName) {
			return GetChar(doc, elementName, null);
		}

		public static DateTime? GetDateTime(this XDocument doc, XName elementName, XmlDateTimeSerializationMode mode, DateTime? @default) {
			string result = Get(doc, elementName, null, null);
			if (result == null) {
				return @default;
			}
			return XmlConvert.ToDateTime(result, mode);
		}

		public static DateTime? GetDateTime(this XDocument doc, XName elementName, XmlDateTimeSerializationMode mode) {
			return GetDateTime(doc, elementName, mode, null);
		}

		public static DateTimeOffset? GetDateTimeOffset(this XDocument doc, XName elementName, DateTimeOffset? @default) {
			string result = Get(doc, elementName, null, null);
			if (result == null) {
				return @default;
			}
			return XmlConvert.ToDateTimeOffset(result);
		}

		public static DateTimeOffset? GetDateTimeOffset(this XDocument doc, XName elementName) {
			return GetDateTimeOffset(doc, elementName, null);
		}

		public static Decimal? GetDecimal(this XDocument doc, XName elementName, decimal? @default) {
			string result = Get(doc, elementName, null, null);
			if (result == null) {
				return @default;
			}
			return XmlConvert.ToDecimal(result);
		}

		public static Decimal? GetDecimal(this XDocument doc, XName elementName) {
			return GetDecimal(doc, elementName, null);
		}

		public static double? GetDouble(this XDocument doc, XName elementName, double? @default) {
			string result = Get(doc, elementName, null, null);
			if (result == null) {
				return @default;
			}
			return XmlConvert.ToDouble(result);
		}

		public static double? GetDouble(this XDocument doc, XName elementName) {
			return GetDouble(doc, elementName, null);
		}

		public static XElement GetElement(this XDocument doc, XName elementName, CultureInfo culture, bool cultureFallback) {
			if (elementName == null) {
				throw new ArgumentNullException("elementName");
			}
			if (doc != null) {
				XElement root = doc.Root;
				if (root != null) {
					if (culture == null) {
						culture = CultureInfo.InvariantCulture;
					}
					do {
						string cultureId = culture.ToString();
						foreach (XElement element in root.Elements(elementName)) {
							XAttribute lang = element.Attribute(XmlLang);
							if ((lang == null) ? (cultureId.Length == 0) : (lang.Value == cultureId)) {
								return element;
							}
						}
						if ((!cultureFallback) || (culture == CultureInfo.InvariantCulture)) {
							culture = null;
						} else {
							culture = culture.Parent;
						}
					} while (culture != null);
				}
			}
			return null;
		}

		public static Guid? GetGuid(this XDocument doc, XName elementName, Guid? @default) {
			string result = Get(doc, elementName, null, null);
			if (result == null) {
				return @default;
			}
			return XmlConvert.ToGuid(result);
		}

		public static Guid? GetGuid(this XDocument doc, XName elementName) {
			return GetGuid(doc, elementName, null);
		}

		public static short? GetInt16(this XDocument doc, XName elementName, short? @default) {
			string result = Get(doc, elementName, null, null);
			if (result == null) {
				return @default;
			}
			return XmlConvert.ToInt16(result);
		}

		public static short? GetInt16(this XDocument doc, XName elementName) {
			return GetInt16(doc, elementName, null);
		}

		public static int? GetInt32(this XDocument doc, XName elementName, int? @default) {
			string result = Get(doc, elementName, null, null);
			if (result == null) {
				return @default;
			}
			return XmlConvert.ToInt32(result);
		}

		public static int? GetInt32(this XDocument doc, XName elementName) {
			return GetInt32(doc, elementName, null);
		}

		public static long? GetInt64(this XDocument doc, XName elementName, long? @default) {
			string result = Get(doc, elementName, null, null);
			if (result == null) {
				return @default;
			}
			return XmlConvert.ToInt64(result);
		}

		public static long? GetInt64(this XDocument doc, XName elementName) {
			return GetInt64(doc, elementName, null);
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

		public static sbyte? GetSByte(this XDocument doc, XName elementName, sbyte? @default) {
			string result = Get(doc, elementName, null, null);
			if (result == null) {
				return @default;
			}
			return XmlConvert.ToSByte(result);
		}

		public static sbyte? GetSByte(this XDocument doc, XName elementName) {
			return GetSByte(doc, elementName, null);
		}

		public static float? GetSingle(this XDocument doc, XName elementName, float? @default) {
			string result = Get(doc, elementName, null, null);
			if (result == null) {
				return @default;
			}
			return XmlConvert.ToSingle(result);
		}

		public static float? GetSingle(this XDocument doc, XName elementName) {
			return GetSingle(doc, elementName, null);
		}

		public static TimeSpan? GetTimeSpan(this XDocument doc, XName elementName, TimeSpan? @default) {
			string result = Get(doc, elementName, null, null);
			if (result == null) {
				return @default;
			}
			return XmlConvert.ToTimeSpan(result);
		}

		public static TimeSpan? GetTimeSpan(this XDocument doc, XName elementName) {
			return GetTimeSpan(doc, elementName, null);
		}

		public static ushort? GetUInt16(this XDocument doc, XName elementName, ushort? @default) {
			string result = Get(doc, elementName, null, null);
			if (result == null) {
				return @default;
			}
			return XmlConvert.ToUInt16(result);
		}

		public static ushort? GetUInt16(this XDocument doc, XName elementName) {
			return GetUInt16(doc, elementName, null);
		}

		public static uint? GetUInt32(this XDocument doc, XName elementName, uint? @default) {
			string result = Get(doc, elementName, null, null);
			if (result == null) {
				return @default;
			}
			return XmlConvert.ToUInt32(result);
		}

		public static uint? GetUInt32(this XDocument doc, XName elementName) {
			return GetUInt32(doc, elementName, null);
		}

		public static ulong? GetUInt64(this XDocument doc, XName elementName, ulong? @default) {
			string result = Get(doc, elementName, null, null);
			if (result == null) {
				return @default;
			}
			return XmlConvert.ToUInt64(result);
		}

		public static ulong? GetUInt64(this XDocument doc, XName elementName) {
			return GetUInt64(doc, elementName, null);
		}

		public static bool IsSet(this XDocument doc, XName elementName) {
			return GetElement(doc, elementName, null, false) != null;
		}

		public static XElement NextMedadataSibling(this XElement element) {
			if (element == null) {
				throw new ArgumentNullException("element");
			}
			XAttribute lang = element.Attribute(XmlLang);
			string cultureId = (lang != null) ? lang.Value : string.Empty;
			XName elementName = element.Name;
			for (XNode node = element.NextNode; node != null; node = node.NextNode) {
				element = node as XElement;
				if ((element != null) && (element.Name == elementName)) {
					lang = element.Attribute(XmlLang);
					if ((lang == null) ? (cultureId.Length == 0) : (lang.Value == cultureId)) {
						return element;
					}
				}
			}
			return null;
		}

		public static void Set(this XDocument doc, XName elementName, bool? value) {
			doc.Set(elementName, null, value.HasValue ? XmlConvert.ToString(value.Value) : null);
		}

		public static void Set(this XDocument doc, XName elementName, byte? value) {
			doc.Set(elementName, null, value.HasValue ? XmlConvert.ToString(value.Value) : null);
		}

		public static void Set(this XDocument doc, XName elementName, char? value) {
			doc.Set(elementName, null, value.HasValue ? XmlConvert.ToString(value.Value) : null);
		}

		public static void Set(this XDocument doc, XName elementName, DateTime? value, XmlDateTimeSerializationMode mode) {
			doc.Set(elementName, null, value.HasValue ? XmlConvert.ToString(value.Value, mode) : null);
		}

		public static void Set(this XDocument doc, XName elementName, DateTimeOffset? value) {
			doc.Set(elementName, null, value.HasValue ? XmlConvert.ToString(value.Value) : null);
		}

		public static void Set(this XDocument doc, XName elementName, decimal? value) {
			doc.Set(elementName, null, value.HasValue ? XmlConvert.ToString(value.Value) : null);
		}

		public static void Set(this XDocument doc, XName elementName, double? value) {
			doc.Set(elementName, null, value.HasValue ? XmlConvert.ToString(value.Value) : null);
		}

		public static void Set(this XDocument doc, XName elementName, Guid? value) {
			doc.Set(elementName, null, value.HasValue ? XmlConvert.ToString(value.Value) : null);
		}

		public static void Set(this XDocument doc, XName elementName, short? value) {
			doc.Set(elementName, null, value.HasValue ? XmlConvert.ToString(value.Value) : null);
		}

		public static void Set(this XDocument doc, XName elementName, int? value) {
			doc.Set(elementName, null, value.HasValue ? XmlConvert.ToString(value.Value) : null);
		}

		public static void Set(this XDocument doc, XName elementName, long? value) {
			doc.Set(elementName, null, value.HasValue ? XmlConvert.ToString(value.Value) : null);
		}

		public static void Set(this XDocument doc, XName elementName, sbyte? value) {
			doc.Set(elementName, null, value.HasValue ? XmlConvert.ToString(value.Value) : null);
		}

		public static void Set(this XDocument doc, XName elementName, float? value) {
			doc.Set(elementName, null, value.HasValue ? XmlConvert.ToString(value.Value) : null);
		}

		public static void Set(this XDocument doc, XName elementName, string value) {
			doc.Set(elementName, null, value);
		}

		public static void Set(this XDocument doc, XName elementName, TimeSpan? value) {
			doc.Set(elementName, null, value.HasValue ? XmlConvert.ToString(value.Value) : null);
		}

		public static void Set(this XDocument doc, XName elementName, ushort? value) {
			doc.Set(elementName, null, value.HasValue ? XmlConvert.ToString(value.Value) : null);
		}

		public static void Set(this XDocument doc, XName elementName, uint? value) {
			doc.Set(elementName, null, value.HasValue ? XmlConvert.ToString(value.Value) : null);
		}

		public static void Set(this XDocument doc, XName elementName, ulong? value) {
			doc.Set(elementName, null, value.HasValue ? XmlConvert.ToString(value.Value) : null);
		}

		public static void Set(this XDocument doc, XName elementName, CultureInfo culture, string value) {
			XElement element = doc.GetElement(elementName, culture, false);
			if (element == null) {
				if (value == null) {
					return;
				}
				element = new XElement(elementName, value);
				if ((culture != null) && (culture != CultureInfo.InvariantCulture)) {
					element.SetAttributeValue(XmlLang, culture.ToString());
				}
				doc.GetRoot().Add(element);
			} else {
				if (value != null) {
					element.RemoveAll();
					element.Value = value;
					element = element.NextMedadataSibling();
				}
				while (element != null) {
					XElement next = element.NextMedadataSibling();
					element.Remove();
					element = next;
				}
			}
		}

		public static void Unset(this XDocument doc, XName elementName) {
			Set(doc, elementName, null, null);
		}
	}
}
