using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Procon.Core.Shared {
    /// <summary>
    /// A single parameter to be used in a command
    /// </summary>
    public interface ICommandParameter {
        /// <summary>
        /// The data stored in this parameter.
        /// </summary>
        ICommandData Data { get; set; }

        /// <summary>
        /// Checks if this parameter has a specific data type.
        /// </summary>
        /// <returns></returns>
        bool HasOne<T>(bool convert = true);

        /// <summary>
        /// Checks if this parameter has a specific data type.
        /// </summary>
        /// <returns></returns>
        bool HasOne(Type t, bool convert = true);

        /// <summary>
        /// Checks if more than one exists of a type, specifying that a collection of objects is currently set.
        /// </summary>
        /// <returns></returns>
        bool HasMany<T>(bool convert = true);

        /// <summary>
        /// Checks if more than one exists of a type, specifying that a collection of objects is currently set.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="convert">True if a specific value of that type cannot be found then a conversion should be looked for.</param>
        /// <returns></returns>
        bool HasMany(Type t, bool convert = true);

        /// <summary>
        /// Fetches the first matching object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T First<T>(bool convert = true);

        /// <summary>
        /// Fetches the first matching object
        /// </summary>
        /// <returns></returns>
        Object First(Type t, bool convert = true);

        /// <summary>
        /// Fetches all of the objects of the specified type.
        /// </summary>
        /// <returns></returns>
        Object All(Type t, bool convert = true);

        /// <summary>
        /// Fetches all of the objects of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        List<T> All<T>();
    }
}
