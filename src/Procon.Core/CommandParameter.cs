using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Procon.Core {

    [Serializable]
    public sealed class CommandParameter {

        private static readonly Dictionary<Type, PropertyInfo> TypeReferences = new Dictionary<Type, PropertyInfo>();

        /// <summary>
        /// The data stored in this parameter.
        /// </summary>
        public CommandData Data { get; set; }

        static CommandParameter() {
            foreach (PropertyInfo property in typeof(CommandData).GetProperties()) {
                Type[] genericArgumentTypes = property.PropertyType.GetGenericArguments();

                if (genericArgumentTypes.Length == 1) {
                    CommandParameter.TypeReferences.Add(genericArgumentTypes[0], property);
                }
            }
        }

        public CommandParameter() {
            this.Data = new CommandData();
        }

        /// <summary>
        /// Checks if this parameter has a specific data type.
        /// </summary>
        /// <returns></returns>
        public bool HasOne<T>(bool convert = true) {
            return this.HasOne(typeof(T), convert);
        }

        /// <summary>
        /// Checks if this parameter has a specific data type.
        /// </summary>
        /// <returns></returns>
        public bool HasOne(Type t, bool convert = true) {
            bool hasOne = false;

            if (CommandParameter.TypeReferences.ContainsKey(t) == true && CommandParameter.TypeReferences[t].GetValue(this.Data, null) != null) {
                hasOne = true;
            }
            else if (convert == true) {
                if (t.IsEnum == true && this.HasOne<String>() == true && CommandParameter.ConvertToEnum(t, this.First<String>()) != null) {
                    hasOne = true;
                }
                else if (this.HasOne<String>(false) == true) {
                    try {
                        hasOne = System.Convert.ChangeType(this.First<String>(false), t) != null;
                    }
                    catch {
                        hasOne = false;
                    }
                }
            }

            return hasOne;
        }

        /// <summary>
        /// Checks if more than one exists of a type, specifying that a collection of objects is currently set.
        /// </summary>
        /// <returns></returns>
        public bool HasMany<T>(bool convert = true) {
            return this.HasMany(typeof(T), convert);
        }

        /// <summary>
        /// Checks if more than one exists of a type, specifying that a collection of objects is currently set.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="convert">True if a specific value of that type cannot be found then a conversion should be looked for.</param>
        /// <returns></returns>
        public bool HasMany(Type t, bool convert = true) {
            bool hasMany = false;

            if (CommandParameter.TypeReferences.ContainsKey(t) == true) {
                Object collection = CommandParameter.TypeReferences[t].GetValue(this.Data, null);

                if (collection.GetType().GetProperty("Count") != null) {
                    hasMany = (int) collection.GetType().GetProperty("Count").GetValue(collection, null) > 1;
                }
            }
            else if (convert == true) {
                // If we're looking for an enum
                if (t.IsEnum == true) {
                    List<String> strings = this.All<String>();

                    hasMany = strings.Select(s => CommandParameter.ConvertToEnum(t, s)).Count(e => e != null) == strings.Count;
                }
                // Else can we convert a list of strings to their type?
                else {
                    List<String> strings = this.All<String>();

                    try {
                        hasMany = strings.Select(s => System.Convert.ChangeType(s, t)).Count(e => e != null) == strings.Count;
                    }
                    catch {
                        hasMany = false;
                    }
                }
            }


            return hasMany;
        }

        /// <summary>
        /// Fetches the first matching object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T First<T>(bool convert = true) {
            Object value = this.First(typeof(T), convert);

            return value != null ? (T)value : default(T);
        }

        /// <summary>
        /// Fetches the first matching object
        /// </summary>
        /// <returns></returns>
        public Object First(Type t, bool convert = true) {
            Object result = null;

            if (CommandParameter.TypeReferences.ContainsKey(t) == true) {
                object collection = CommandParameter.TypeReferences[t].GetValue(this.Data, null);

                if (collection.GetType().GetProperty("Count") != null) {
                    if ((int)collection.GetType().GetProperty("Count").GetValue(collection, null) > 0) {
                        String indexerName = ((DefaultMemberAttribute)collection.GetType()
                                        .GetCustomAttributes(typeof(DefaultMemberAttribute),
                                         true)[0]).MemberName;

                        result = collection.GetType().GetProperty(indexerName).GetValue(collection, new Object[] { 0 });
                    }
                }
            }
            else if (convert == true) {
                // If we're looking for an enum
                if (t.IsEnum == true) {
                    // Convert the value, which will be null if no conversion exists.
                    result = CommandParameter.ConvertToEnum(t, this.First<String>());
                }
                    // Else can we convert a list of strings to their type?
                else {
                    try {
                        result = System.Convert.ChangeType(this.First<String>(), t);
                    }
                    catch {
                        result = null;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Fetches all of the objects of the specified type.
        /// </summary>
        /// <returns></returns>
        public Object All(Type t, bool convert = true) {
            Object result = null;

            if (CommandParameter.TypeReferences.ContainsKey(t) == true) {
                result = CommandParameter.TypeReferences[t].GetValue(this.Data, null);
            }
            else if (convert == true) {
                // if we're looking for an enum
                if (t.IsEnum == true) {
                    // Convert the value, which will be null if no conversion exists.
                    result = this.All<String>()
                        .Select(s => CommandParameter.ConvertToEnum(t, s))
                        .Where(e => e != null)
                        .ToList();
                }
                // Else can we convert a list of strings to their type?
                else {
                    try {
                        result = this.All<String>().Select(s => System.Convert.ChangeType(s, t)).Where(e => e != null).ToList();
                    }
                    catch {
                        result = null;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Fetches all of the objects of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> All<T>() {
            List<T> result = new List<T>();

            if (CommandParameter.TypeReferences.ContainsKey(typeof(T)) == true) {
                result = CommandParameter.TypeReferences[typeof(T)].GetValue(this.Data, null) as List<T>;
            }

            return result;
        }
        
        /// <summary>
        /// Parses an enum either directly by assuming the value is a string or by assuming it's a primitive type.
        /// </summary>
        /// <param name="type">The type of enum we need</param>
        /// <param name="value">The value as an object that we need to convert to the enum</param>
        /// <returns>The converted value to enum</returns>
        private static Object ConvertToEnum(Type type, Object value) {
            Object enumValue = null;

            if (type.IsEnum == true && value != null) {
                string stringValue = value as string;

                if (stringValue != null) {
                    if (Enum.IsDefined(type, stringValue) == true) {
                        enumValue = Enum.Parse(type, stringValue);
                    }
                }
                /* This was functional, but I don't beleive we were using it (and I can't see why we would at the moment?)
                else {
                    int? val = (int?)System.Convert.ChangeType(value.ToString(), typeof(int));

                    if (val.HasValue == true && Enum.IsDefined(type, val) == true) {
                        String enumName = Enum.GetName(type, value);

                        if (enumName != null) {
                            enumValue = Enum.Parse(type, enumName);
                        }
                    }
                }
                */
            }

            return enumValue;
        }
    }
}
