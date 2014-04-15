#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Collections.Generic;

namespace Potato.Core.Shared {
    /// <summary>
    /// A single parameter to be used in a command
    /// </summary>
    public interface ICommandParameter {
        /// <summary>
        /// The data stored in this parameter.
        /// </summary>
        ICommandData Data { get; set; }

        /// <summary>
        /// Converts a value to a specific type. Essentially a wrapper for Convert.ChangeType but
        /// checks for other common classes to convert to.
        /// </summary>
        /// <param name="value">The value to be changed</param>
        /// <param name="conversionType">The type to be changed to</param>
        /// <returns>The changed type</returns>
        Object ChangeType(Object value, Type conversionType);

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
