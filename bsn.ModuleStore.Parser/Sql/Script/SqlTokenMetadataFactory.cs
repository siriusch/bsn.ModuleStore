using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace bsn.ModuleStore.Sql.Script {
	internal static class SqlTokenMetadataFactory {
		private interface ISqlTokenEnumeratorGetter<TInstance> {
			IEnumerable<SqlToken> GetEnumerator(TInstance instance);
		}

		private class SqlTokenEnumeratorGetter<TInstance, TItem>: ISqlTokenEnumeratorGetter<TInstance> where TInstance: SqlToken
		                                                                                               where TItem: SqlToken {
			private readonly Func<TInstance, IEnumerable<TItem>> propertyGetter;

			public SqlTokenEnumeratorGetter(Func<TInstance, IEnumerable<TItem>> propertyGetter) {
				this.propertyGetter = propertyGetter;
			}

			public IEnumerable<SqlToken> GetEnumerator(TInstance instance) {
				foreach (TItem item in propertyGetter(instance)) {
					yield return item;
				}
			}
		                                                                                               }

		private abstract class SqlTokenMetadata {
			public abstract IEnumerable<SqlToken> EnumerateTokensUntyped(SqlToken instance);
		}

		private class SqlTokenMetadata<TToken, TTokenBase>: SqlTokenMetadata, ISqlTokenMetadata<TToken> where TToken: TTokenBase
		                                                                                                where TTokenBase: SqlToken {
			private static IEnumerable<Type> GetInterfaces(Type type) {
				if (type.IsInterface) {
					yield return type;
				}
				foreach (Type @interface in type.GetInterfaces()) {
					yield return @interface;
				}
			}

			private readonly ISqlTokenMetadata<TTokenBase> baseMetadata;
			private readonly ISqlTokenEnumeratorGetter<TToken>[] enumeratorGetters;
			private readonly Func<TToken, SqlToken>[] instanceGetters;

			public SqlTokenMetadata(ISqlTokenMetadata<TTokenBase> baseMetadata, ICollection<string> checkFieldErrors) {
				this.baseMetadata = baseMetadata;
				Dictionary<string, bool> propertyNames = null;
				if (checkFieldErrors != null) {
					propertyNames = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
				}
				List<Func<TToken, SqlToken>> instances = new List<Func<TToken, SqlToken>>();
				List<ISqlTokenEnumeratorGetter<TToken>> enumerators = new List<ISqlTokenEnumeratorGetter<TToken>>();
				foreach (PropertyInfo property in typeof(TToken).GetProperties(BindingFlags.DeclaredOnly|BindingFlags.Instance|BindingFlags.Public)) {
					MethodInfo propertyGetter = property.GetGetMethod();
					if (propertyGetter != null) {
						if (typeof(SqlToken).IsAssignableFrom(property.PropertyType)) {
							if (propertyNames != null) {
								propertyNames.Add(property.Name, false);
							}
							Type delegateType = typeof(Func<,>).MakeGenericType(typeof(TToken), typeof(SqlToken));
							instances.Add((Func<TToken, SqlToken>)Delegate.CreateDelegate(delegateType, propertyGetter));
						} else {
							foreach (Type @interface in GetInterfaces(property.PropertyType)) {
								if (@interface.IsGenericType && (@interface.GetGenericTypeDefinition() == typeof(IEnumerable<>))) {
									Type enumerationType = @interface.GetGenericArguments()[0];
									if (typeof(SqlToken).IsAssignableFrom(enumerationType)) {
										if (propertyNames != null) {
											propertyNames.Add(property.Name, true);
										}
										Type delegateType = typeof(Func<,>).MakeGenericType(typeof(TToken), @interface);
										Type getterType = typeof(SqlTokenEnumeratorGetter<,>).MakeGenericType(typeof(TToken), enumerationType);
										ISqlTokenEnumeratorGetter<TToken> getter = (ISqlTokenEnumeratorGetter<TToken>)Activator.CreateInstance(getterType, Delegate.CreateDelegate(delegateType, propertyGetter));
										enumerators.Add(getter);
										break;
									}
								}
							}
						}
					}
				}
				if (propertyNames != null) {
					CheckFieldNames(propertyNames, checkFieldErrors);
				}
				enumeratorGetters = enumerators.ToArray();
				instanceGetters = instances.ToArray();
			}

			public override IEnumerable<SqlToken> EnumerateTokensUntyped(SqlToken instance) {
				return EnumerateTokens((TToken)instance);
			}

			private void CheckFieldNames(Dictionary<string, bool> propertyNames, ICollection<string> checkFieldErrors) {
				foreach (FieldInfo field in typeof(TToken).GetFields(BindingFlags.DeclaredOnly|BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.Public)) {
					if (typeof(SqlToken).IsAssignableFrom(field.FieldType)) {
						if (!propertyNames.Remove(field.Name)) {
							checkFieldErrors.Add(string.Format("Missing instance property: {0}", field.Name));
						}
					} else {
						foreach (Type @interface in field.FieldType.GetInterfaces()) {
							if (@interface.IsGenericType && (@interface.GetGenericTypeDefinition() == typeof(IEnumerable<>))) {
								Type enumerationType = @interface.GetGenericArguments()[0];
								if (typeof(SqlToken).IsAssignableFrom(enumerationType)) {
									if (!propertyNames.Remove(field.Name)) {
										checkFieldErrors.Add(string.Format("Missing enumerator property: {0}", field.Name));
									}
									break;
								}
							}
						}
					}
				}
				foreach (KeyValuePair<string, bool> propertyName in propertyNames) {
					if (propertyName.Value) {
						checkFieldErrors.Add(string.Format("Instance property without field: {0}", propertyName));
					} else {
						checkFieldErrors.Add(string.Format("Enumerator property without field: {0}", propertyName));
					}
				}
			}

			public IEnumerable<SqlToken> EnumerateTokens(TToken instance) {
				if (instance != null) {
					if (baseMetadata != null) {
						foreach (SqlToken token in baseMetadata.EnumerateTokens(instance)) {
							yield return token;
						}
					}
					foreach (Func<TToken, SqlToken> instanceGetter in instanceGetters) {
						SqlToken token = instanceGetter(instance);
						if (token != null) {
							yield return token;
						}
					}
					foreach (ISqlTokenEnumeratorGetter<TToken> enumeratorGetter in enumeratorGetters) {
						foreach (SqlToken token in enumeratorGetter.GetEnumerator(instance)) {
							if (token != null) {
								yield return token;
							}
						}
					}
				}
			}
		                                                                                                }

		private static readonly Dictionary<Type, SqlTokenMetadata> tokenMetadata = new Dictionary<Type, SqlTokenMetadata>();

		internal static void CheckFieldsAndProperties() {
			List<string> errors = new List<string>();
			List<string> localErrors = new List<string>();
			lock (tokenMetadata) {
				tokenMetadata.Clear();
				int typeCount = 0;
				foreach (Type type in typeof(SqlToken).Assembly.GetTypes()) {
					if ((!type.IsGenericTypeDefinition) && typeof(SqlToken).IsAssignableFrom(type)) {
						typeCount++;
						localErrors.Clear();
						GetTokenMetadataInternal(type, localErrors);
						foreach (string error in localErrors) {
							errors.Add(string.Format("{0}: {1}", type, error));
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

		public static IEnumerable<SqlToken> GetInnerTokens(this SqlToken token) {
			if (token != null) {
				Queue<SqlToken> itemsToProcess = new Queue<SqlToken>();
				itemsToProcess.Enqueue(token);
				do {
					token = itemsToProcess.Dequeue();
					Debug.Assert(token != null);
					foreach (SqlToken innerToken in GetTokenMetadataInternal(token.GetType(), null).EnumerateTokensUntyped(token)) {
						yield return innerToken;
						itemsToProcess.Enqueue(innerToken);
					}
				} while (itemsToProcess.Count > 0);
			}
		}

		private static SqlTokenMetadata GetTokenMetadataInternal(Type tokenType, ICollection<string> checkFieldErrors) {
			lock (tokenMetadata) {
				SqlTokenMetadata value;
				if (tokenMetadata.TryGetValue(tokenType, out value)) {
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
				Type[] constructorParameters = new[] {typeof(ISqlTokenMetadata<>).MakeGenericType(baseType), typeof(ICollection<string>)};
				ConstructorInfo constructor = typeof(SqlTokenMetadata<,>).MakeGenericType(tokenType, baseType).GetConstructor(constructorParameters);
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
