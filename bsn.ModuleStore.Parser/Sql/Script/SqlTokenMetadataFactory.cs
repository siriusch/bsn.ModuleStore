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
using System.Diagnostics;
using System.Reflection;

namespace bsn.ModuleStore.Sql.Script {
	internal static class SqlTokenMetadataFactory {
		internal abstract class SqlTokenMetadata {
			private readonly bool isCommonTableExpressionScope;

			protected SqlTokenMetadata(Type type) {
				isCommonTableExpressionScope = type.GetCustomAttributes(typeof(CommonTableExpressionScopeAttribute), true).Length > 0;
			}

			public bool IsCommonTableExpressionScope => isCommonTableExpressionScope;

			public abstract IEnumerable<SqlToken> EnumerateTokensUntyped(SqlToken instance, Type skipNestedOfType);
		}

		private interface ISqlTokenEnumeratorGetter<TInstance> {
			bool IsPriority {
				get;
			}

			IEnumerable<SqlToken> GetEnumerator(TInstance instance);
		}

		private class SqlTokenEnumeratorGetter<TInstance, TItem>: ISqlTokenEnumeratorGetter<TInstance> where TInstance: SqlToken
				where TItem: SqlToken {
			private readonly bool isPriority;
			private readonly Func<TInstance, IEnumerable<TItem>> propertyGetter;

			public SqlTokenEnumeratorGetter(Func<TInstance, IEnumerable<TItem>> propertyGetter, bool isPriority) {
				this.propertyGetter = propertyGetter;
				this.isPriority = isPriority;
			}

			public IEnumerable<SqlToken> GetEnumerator(TInstance instance) {
				foreach (var item in propertyGetter(instance)) {
					yield return item;
				}
			}

			public bool IsPriority => isPriority;
		}

		private class SqlTokenMetadata<TToken, TTokenBase>: SqlTokenMetadata, ISqlTokenMetadata<TToken> where TToken: TTokenBase
				where TTokenBase: SqlToken {
			private static IEnumerable<Type> GetInterfaces(Type type) {
				if (type.IsInterface) {
					yield return type;
				}
				foreach (var @interface in type.GetInterfaces()) {
					yield return @interface;
				}
			}

			private static bool ShouldYieldToken(SqlToken token, Type skipNestedOfType) {
				if (token != null) {
					return (skipNestedOfType == null) || skipNestedOfType.IsAssignableFrom(token.GetType());
				}
				return false;
			}

			private readonly ISqlTokenMetadata<TTokenBase> baseMetadata;
			private readonly IEnumerable<ISqlTokenEnumeratorGetter<TToken>> enumeratorGetters;
			private readonly IEnumerable<Func<TToken, SqlToken>> instanceGetters;

			public SqlTokenMetadata(ISqlTokenMetadata<TTokenBase> baseMetadata, ICollection<string> checkFieldErrors): base(typeof(TToken)) {
				this.baseMetadata = baseMetadata;
				Dictionary<string, bool> propertyNames = null;
				if (checkFieldErrors != null) {
					propertyNames = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
				}
				var instances = new List<Func<TToken, SqlToken>>();
				var enumerators = new List<ISqlTokenEnumeratorGetter<TToken>>();
				foreach (var property in typeof(TToken).GetProperties(BindingFlags.DeclaredOnly|BindingFlags.Instance|BindingFlags.Public)) {
					var propertyGetter = property.GetGetMethod();
					if (propertyGetter != null) {
						if (typeof(SqlToken).IsAssignableFrom(property.PropertyType)) {
							if ((propertyNames != null) && (!property.IsDefined(typeof(SkipConsistencyCheckAttribute), true))) {
								propertyNames.Add(property.Name, false);
							}
							var delegateType = typeof(Func<,>).MakeGenericType(typeof(TToken), typeof(SqlToken));
							var isPriority = typeof(QueryOptions).IsAssignableFrom(property.PropertyType);
							var getter = (Func<TToken, SqlToken>)Delegate.CreateDelegate(delegateType, propertyGetter);
							if (isPriority) {
								instances.Insert(0, getter);
							} else {
								instances.Add(getter);
							}
						} else {
							foreach (var @interface in GetInterfaces(property.PropertyType)) {
								if (@interface.IsGenericType && (@interface.GetGenericTypeDefinition() == typeof(IEnumerable<>))) {
									var enumerationType = @interface.GetGenericArguments()[0];
									if (typeof(SqlToken).IsAssignableFrom(enumerationType)) {
										if ((propertyNames != null) && (!property.IsDefined(typeof(SkipConsistencyCheckAttribute), true))) {
											propertyNames.Add(property.Name, true);
										}
										var delegateType = typeof(Func<,>).MakeGenericType(typeof(TToken), @interface);
										var getterType = typeof(SqlTokenEnumeratorGetter<,>).MakeGenericType(typeof(TToken), enumerationType);
										var isPriority = typeof(CommonTableExpression).IsAssignableFrom(enumerationType);
										var getter = (ISqlTokenEnumeratorGetter<TToken>)Activator.CreateInstance(getterType, Delegate.CreateDelegate(delegateType, propertyGetter), isPriority);
										if (isPriority) {
											enumerators.Insert(0, getter);
										} else {
											enumerators.Add(getter);
										}
										break;
									}
								}
							}
						}
					}
				}
				if ((propertyNames != null) && (!typeof(TToken).IsDefined(typeof(SkipConsistencyCheckAttribute), true))) {
					CheckFieldNames(propertyNames, checkFieldErrors);
				}
				enumeratorGetters = enumerators.ToArray();
				instanceGetters = instances.ToArray();
			}

			public override IEnumerable<SqlToken> EnumerateTokensUntyped(SqlToken instance, Type skipNestedOfType) {
				return EnumerateTokens((TToken)instance, skipNestedOfType);
			}

			[Conditional("DEBUG")]
			private void CheckFieldNames(Dictionary<string, bool> propertyNames, ICollection<string> checkFieldErrors) {
				foreach (var field in typeof(TToken).GetFields(BindingFlags.DeclaredOnly|BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.Public)) {
					if (!field.IsDefined(typeof(SkipConsistencyCheckAttribute), true)) {
						if (typeof(SqlToken).IsAssignableFrom(field.FieldType)) {
							if (!propertyNames.Remove(field.Name)) {
								checkFieldErrors.Add($"Missing instance property: {field.Name}");
							}
						} else {
							foreach (var @interface in field.FieldType.GetInterfaces()) {
								if (@interface.IsGenericType && (@interface.GetGenericTypeDefinition() == typeof(IEnumerable<>))) {
									var enumerationType = @interface.GetGenericArguments()[0];
									if (typeof(SqlToken).IsAssignableFrom(enumerationType)) {
										if (!propertyNames.Remove(field.Name)) {
											checkFieldErrors.Add($"Missing enumerator property: {field.Name}");
										}
										break;
									}
								}
							}
						}
					}
				}
				foreach (var propertyName in propertyNames) {
					if (propertyName.Value) {
						checkFieldErrors.Add($"Instance property without field: {propertyName}");
					} else {
						checkFieldErrors.Add($"Enumerator property without field: {propertyName}");
					}
				}
			}

			public IEnumerable<SqlToken> EnumerateTokens(TToken instance, Type skipNestedOfType) {
				if (instance != null) {
					using (var enumerators = enumeratorGetters.GetEnumerator()) {
						var hasEnumerator = enumerators.MoveNext();
						while (hasEnumerator) {
							var current = enumerators.Current;
							Debug.Assert(current != null);
							if (current.IsPriority) {
								foreach (var token in current.GetEnumerator(instance)) {
									if (ShouldYieldToken(token, skipNestedOfType)) {
										yield return token;
									}
								}
							} else {
								break;
							}
							hasEnumerator = enumerators.MoveNext();
						}
						if (baseMetadata != null) {
							foreach (var token in baseMetadata.EnumerateTokens(instance, skipNestedOfType)) {
								yield return token;
							}
						}
						foreach (var instanceGetter in instanceGetters) {
							var token = instanceGetter(instance);
							if (ShouldYieldToken(token, skipNestedOfType)) {
								yield return token;
							}
						}
						if (hasEnumerator) {
							do {
								var current = enumerators.Current;
								Debug.Assert(current != null);
								foreach (var token in current.GetEnumerator(instance)) {
									if (ShouldYieldToken(token, skipNestedOfType)) {
										yield return token;
									}
								}
							} while (enumerators.MoveNext());
						}
					}
				}
			}
		}

		private static readonly Dictionary<Type, SqlTokenMetadata> tokenMetadata = new Dictionary<Type, SqlTokenMetadata>();

		[Conditional("DEBUG")]
		internal static void CheckFieldsAndProperties() {
			var errors = new List<string>();
			var localErrors = new List<string>();
			lock (tokenMetadata) {
				tokenMetadata.Clear();
				var typeCount = 0;
				foreach (var type in typeof(SqlToken).Assembly.GetTypes()) {
					if ((!type.IsGenericTypeDefinition) && typeof(SqlToken).IsAssignableFrom(type)) {
						typeCount++;
						localErrors.Clear();
						GetTokenMetadataInternal(type, localErrors);
						foreach (var error in localErrors) {
							errors.Add($"{type}: {error}");
						}
					}
				}
				Trace.Write(typeCount, "Checked types");
			}
			if (errors.Count > 0) {
				throw new InvalidOperationException("Consistency error:"+Environment.NewLine+string.Join(Environment.NewLine, errors.ToArray()));
			}
		}

		public static ISqlTokenMetadata<TToken> GetTokenMetadata<TToken>() where TToken: SqlToken {
			return (ISqlTokenMetadata<TToken>)GetTokenMetadataInternal(typeof(TToken), null);
		}

		internal static SqlTokenMetadata GetTokenMetadataInternal(Type tokenType, ICollection<string> checkFieldErrors) {
			lock (tokenMetadata) {
				if (tokenMetadata.TryGetValue(tokenType, out var value)) {
					return value;
				}
				//Debug.WriteLine("Getting token metadata: "+tokenType.FullName);
				Type baseType;
				object baseMetadata;
				if (tokenType == typeof(SqlToken)) {
					baseType = typeof(SqlToken);
					baseMetadata = null;
				} else {
					baseType = tokenType.BaseType;
					//Debug.Write("--> ");
					baseMetadata = GetTokenMetadataInternal(baseType, checkFieldErrors);
				}
				var constructorParameters = new[] {typeof(ISqlTokenMetadata<>).MakeGenericType(baseType), typeof(ICollection<string>)};
				var constructor = typeof(SqlTokenMetadata<,>).MakeGenericType(tokenType, baseType).GetConstructor(constructorParameters);
				try {
					value = (SqlTokenMetadata)constructor.Invoke(new[] {baseMetadata, checkFieldErrors});
				} catch (Exception ex) {
					Debug.WriteLine(ex);
					throw;
				}
				//Debug.WriteLine("OK", tokenType.FullName);
				tokenMetadata.Add(tokenType, value);
				return value;
			}
		}
	}
}
