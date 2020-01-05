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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace bsn.ModuleStore.Mapper {
	public static class MetadataExtension {
		public static readonly XName XmlLang = XName.Get("lang", "http://www.w3.org/XML/1998/namespace");

		public static void Add(this XDocument that, XName elementName, string value) {
			if (that == null) {
				throw new ArgumentNullException(nameof(that));
			}
			if (elementName == null) {
				throw new ArgumentNullException(nameof(elementName));
			}
			if (value == null) {
				throw new ArgumentNullException(nameof(value));
			}
			that.GetRoot().Add(new XElement(elementName, value));
		}

		public static XDocument Clone(this XDocument that) {
			if (that == null) {
				return new XDocument();
			}
			return new XDocument(that);
		}

		public static string Get(this XDocument that, XName elementName) {
			return Get(that, elementName, null, string.Empty);
		}

		public static string Get(this XDocument that, XName elementName, string @default) {
			return Get(that, elementName, null, @default);
		}

		public static string Get(this XDocument that, XName elementName, CultureInfo culture) {
			return Get(that, elementName, culture, string.Empty);
		}

		public static string Get(this XDocument that, XName elementName, CultureInfo culture, string @default) {
			var element = that.GetElement(elementName, culture, true);
			if (element != null) {
				return element.Value;
			}
			return @default;
		}

		public static T? Get<T>(this XDocument that, XName elementName) where T: struct, IComparable, IFormattable, IConvertible {
			return that.Get<T>(elementName, null);
		}

		public static T? Get<T>(this XDocument that, XName elementName, CultureInfo culture) where T: struct, IComparable, IFormattable, IConvertible {
			var value = that.Get(elementName, culture, null);
			if (!string.IsNullOrEmpty(value)) {
				if (typeof(T).IsEnum) {
					return (T)Enum.Parse(typeof(T), value);
				}
				return (T)(((IConvertible)value).ToType(typeof(T), CultureInfo.InvariantCulture));
			}
			return default(T?);
		}

		public static IEnumerable<XElement> GetAll(this XDocument that) {
			if (that != null) {
				var root = that.Root;
				if (root != null) {
					return root.Elements();
				}
			}
			return Enumerable.Empty<XElement>();
		}

		public static bool? GetBoolean(this XDocument that, XName elementName, bool? @default) {
			var result = Get(that, elementName, null, null);
			if (result == null) {
				return @default;
			}
			if (bool.TryParse(result, out var resultValue)) {
				return resultValue;
			}
			return XmlConvert.ToBoolean(result);
		}

		public static bool? GetBoolean(this XDocument that, XName elementName) {
			return GetBoolean(that, elementName, null);
		}

		public static byte? GetByte(this XDocument that, XName elementName, byte? @default) {
			var result = Get(that, elementName, null, null);
			if (result == null) {
				return @default;
			}
			return XmlConvert.ToByte(result);
		}

		public static byte? GetByte(this XDocument that, XName elementName) {
			return GetByte(that, elementName, null);
		}

		public static char? GetChar(this XDocument that, XName elementName, char? @default) {
			var result = Get(that, elementName, null, null);
			if (result == null) {
				return @default;
			}
			return XmlConvert.ToChar(result);
		}

		public static char? GetChar(this XDocument that, XName elementName) {
			return GetChar(that, elementName, null);
		}

		public static DateTime? GetDateTime(this XDocument that, XName elementName, XmlDateTimeSerializationMode mode, DateTime? @default) {
			var result = Get(that, elementName, null, null);
			if (result == null) {
				return @default;
			}
			return XmlConvert.ToDateTime(result, mode);
		}

		public static DateTime? GetDateTime(this XDocument that, XName elementName, XmlDateTimeSerializationMode mode) {
			return GetDateTime(that, elementName, mode, null);
		}

		public static DateTimeOffset? GetDateTimeOffset(this XDocument that, XName elementName, DateTimeOffset? @default) {
			var result = Get(that, elementName, null, null);
			if (result == null) {
				return @default;
			}
			return XmlConvert.ToDateTimeOffset(result);
		}

		public static DateTimeOffset? GetDateTimeOffset(this XDocument that, XName elementName) {
			return GetDateTimeOffset(that, elementName, null);
		}

		public static Decimal? GetDecimal(this XDocument that, XName elementName, decimal? @default) {
			var result = Get(that, elementName, null, null);
			if (result == null) {
				return @default;
			}
			return XmlConvert.ToDecimal(result);
		}

		public static Decimal? GetDecimal(this XDocument that, XName elementName) {
			return GetDecimal(that, elementName, null);
		}

		public static double? GetDouble(this XDocument that, XName elementName, double? @default) {
			var result = Get(that, elementName, null, null);
			if (result == null) {
				return @default;
			}
			return XmlConvert.ToDouble(result);
		}

		public static double? GetDouble(this XDocument that, XName elementName) {
			return GetDouble(that, elementName, null);
		}

		public static XElement GetElement(this XDocument that, XName elementName, CultureInfo culture, bool cultureFallback) {
			if (elementName == null) {
				throw new ArgumentNullException(nameof(elementName));
			}
			if (that != null) {
				var root = that.Root;
				if (root != null) {
					if (culture == null) {
						culture = CultureInfo.InvariantCulture;
					}
					do {
						var cultureId = culture.ToString();
						foreach (var element in root.Elements(elementName)) {
							var lang = element.Attribute(XmlLang);
							if ((lang == null) ? (cultureId.Length == 0) : (lang.Value == cultureId)) {
								if (string.IsNullOrEmpty(element.Value) && cultureFallback) {
									break;
								}
								return element;
							}
						}
						if ((!cultureFallback) || culture.Equals(CultureInfo.InvariantCulture)) {
							culture = null;
						} else {
							culture = culture.Parent;
						}
					} while (culture != null);
				}
			}
			return null;
		}

		public static Guid? GetGuid(this XDocument that, XName elementName, Guid? @default) {
			var result = Get(that, elementName, null, null);
			if (result == null) {
				return @default;
			}
			return XmlConvert.ToGuid(result);
		}

		public static Guid? GetGuid(this XDocument that, XName elementName) {
			return GetGuid(that, elementName, null);
		}

		public static short? GetInt16(this XDocument that, XName elementName, short? @default) {
			var result = Get(that, elementName, null, null);
			if (result == null) {
				return @default;
			}
			return XmlConvert.ToInt16(result);
		}

		public static short? GetInt16(this XDocument that, XName elementName) {
			return GetInt16(that, elementName, null);
		}

		public static int? GetInt32(this XDocument that, XName elementName, int? @default) {
			var result = Get(that, elementName, null, null);
			if (result == null) {
				return @default;
			}
			return XmlConvert.ToInt32(result);
		}

		public static int? GetInt32(this XDocument that, XName elementName) {
			return GetInt32(that, elementName, null);
		}

		public static long? GetInt64(this XDocument that, XName elementName, long? @default) {
			var result = Get(that, elementName, null, null);
			if (result == null) {
				return @default;
			}
			return XmlConvert.ToInt64(result);
		}

		public static long? GetInt64(this XDocument that, XName elementName) {
			return GetInt64(that, elementName, null);
		}

		public static XElement GetRoot(this XDocument that) {
			if (that == null) {
				throw new ArgumentNullException(nameof(that));
			}
			var root = that.Root;
			if (root == null) {
				root = new XElement("xml");
				that.Add(root);
			}
			return root;
		}

		public static sbyte? GetSByte(this XDocument that, XName elementName, sbyte? @default) {
			var result = Get(that, elementName, null, null);
			if (result == null) {
				return @default;
			}
			return XmlConvert.ToSByte(result);
		}

		public static sbyte? GetSByte(this XDocument that, XName elementName) {
			return GetSByte(that, elementName, null);
		}

		public static float? GetSingle(this XDocument that, XName elementName, float? @default) {
			var result = Get(that, elementName, null, null);
			if (result == null) {
				return @default;
			}
			return XmlConvert.ToSingle(result);
		}

		public static float? GetSingle(this XDocument that, XName elementName) {
			return GetSingle(that, elementName, null);
		}

		public static TimeSpan? GetTimeSpan(this XDocument that, XName elementName, TimeSpan? @default) {
			var result = Get(that, elementName, null, null);
			if (result == null) {
				return @default;
			}
			return XmlConvert.ToTimeSpan(result);
		}

		public static TimeSpan? GetTimeSpan(this XDocument that, XName elementName) {
			return GetTimeSpan(that, elementName, null);
		}

		public static ushort? GetUInt16(this XDocument that, XName elementName, ushort? @default) {
			var result = Get(that, elementName, null, null);
			if (result == null) {
				return @default;
			}
			return XmlConvert.ToUInt16(result);
		}

		public static ushort? GetUInt16(this XDocument that, XName elementName) {
			return GetUInt16(that, elementName, null);
		}

		public static uint? GetUInt32(this XDocument that, XName elementName, uint? @default) {
			var result = Get(that, elementName, null, null);
			if (result == null) {
				return @default;
			}
			return XmlConvert.ToUInt32(result);
		}

		public static uint? GetUInt32(this XDocument that, XName elementName) {
			return GetUInt32(that, elementName, null);
		}

		public static ulong? GetUInt64(this XDocument that, XName elementName, ulong? @default) {
			var result = Get(that, elementName, null, null);
			if (result == null) {
				return @default;
			}
			return XmlConvert.ToUInt64(result);
		}

		public static ulong? GetUInt64(this XDocument that, XName elementName) {
			return GetUInt64(that, elementName, null);
		}

		public static bool IsSet(this XDocument that, XName elementName) {
			return GetElement(that, elementName, null, false) != null;
		}

		public static XElement NextMedadataSibling(this XElement that) {
			if (that == null) {
				throw new ArgumentNullException(nameof(that));
			}
			var lang = that.Attribute(XmlLang);
			var cultureId = (lang != null) ? lang.Value : string.Empty;
			var elementName = that.Name;
			for (var node = that.NextNode; node != null; node = node.NextNode) {
				that = node as XElement;
				if ((that != null) && (that.Name == elementName)) {
					lang = that.Attribute(XmlLang);
					if ((lang == null) ? (cultureId.Length == 0) : (lang.Value == cultureId)) {
						return that;
					}
				}
			}
			return null;
		}

		public static XElement Set(this XDocument that, XName elementName, bool? value) {
			return that.Set(elementName, null, value.HasValue ? XmlConvert.ToString(value.Value) : null);
		}

		public static XElement Set(this XDocument that, XName elementName, byte? value) {
			return that.Set(elementName, null, value.HasValue ? XmlConvert.ToString(value.Value) : null);
		}

		public static XElement Set(this XDocument that, XName elementName, char? value) {
			return that.Set(elementName, null, value.HasValue ? XmlConvert.ToString(value.Value) : null);
		}

		public static XElement Set(this XDocument that, XName elementName, DateTime? value, XmlDateTimeSerializationMode mode) {
			return that.Set(elementName, null, value.HasValue ? XmlConvert.ToString(value.Value, mode) : null);
		}

		public static XElement Set(this XDocument that, XName elementName, DateTimeOffset? value) {
			return that.Set(elementName, null, value.HasValue ? XmlConvert.ToString(value.Value) : null);
		}

		public static XElement Set(this XDocument that, XName elementName, decimal? value) {
			return that.Set(elementName, null, value.HasValue ? XmlConvert.ToString(value.Value) : null);
		}

		public static XElement Set(this XDocument that, XName elementName, double? value) {
			return that.Set(elementName, null, value.HasValue ? XmlConvert.ToString(value.Value) : null);
		}

		public static XElement Set(this XDocument that, XName elementName, Guid? value) {
			return that.Set(elementName, null, value.HasValue ? XmlConvert.ToString(value.Value) : null);
		}

		public static XElement Set(this XDocument that, XName elementName, short? value) {
			return that.Set(elementName, null, value.HasValue ? XmlConvert.ToString(value.Value) : null);
		}

		public static XElement Set(this XDocument that, XName elementName, int? value) {
			return that.Set(elementName, null, value.HasValue ? XmlConvert.ToString(value.Value) : null);
		}

		public static XElement Set(this XDocument that, XName elementName, long? value) {
			return that.Set(elementName, null, value.HasValue ? XmlConvert.ToString(value.Value) : null);
		}

		public static XElement Set(this XDocument that, XName elementName, sbyte? value) {
			return that.Set(elementName, null, value.HasValue ? XmlConvert.ToString(value.Value) : null);
		}

		public static XElement Set(this XDocument that, XName elementName, float? value) {
			return that.Set(elementName, null, value.HasValue ? XmlConvert.ToString(value.Value) : null);
		}

		public static XElement Set(this XDocument that, XName elementName, string value) {
			return that.Set(elementName, null, value);
		}

		public static XElement Set(this XDocument that, XName elementName, TimeSpan? value) {
			return that.Set(elementName, null, value.HasValue ? XmlConvert.ToString(value.Value) : null);
		}

		public static XElement Set(this XDocument that, XName elementName, ushort? value) {
			return that.Set(elementName, null, value.HasValue ? XmlConvert.ToString(value.Value) : null);
		}

		public static XElement Set(this XDocument that, XName elementName, uint? value) {
			return that.Set(elementName, null, value.HasValue ? XmlConvert.ToString(value.Value) : null);
		}

		public static XElement Set(this XDocument that, XName elementName, ulong? value) {
			return that.Set(elementName, null, value.HasValue ? XmlConvert.ToString(value.Value) : null);
		}

		public static XElement Set(this XDocument that, XName elementName, CultureInfo culture, string value) {
			XElement element = that.GetElement(elementName, culture, false);
			if (element == null) {
				if (value == null) {
					return null;
				}
				element = new XElement(elementName, value);
				if ((culture != null) && (culture != CultureInfo.InvariantCulture)) {
					element.SetAttributeValue(XmlLang, culture.ToString());
				}
				that.GetRoot().Add(element);
				return element;
			}
			var result = element;
			if (value != null) {
				element.RemoveAll();
				element.Value = value;
				element = element.NextMedadataSibling();
			} else {
				result = null;
			}
			while (element != null) {
				XElement next = element.NextMedadataSibling();
				element.Remove();
				element = next;
			}
			return result;
		}

		public static void Unset(this XDocument that, XName elementName) {
			Set(that, elementName, null, null);
		}
	}
}
