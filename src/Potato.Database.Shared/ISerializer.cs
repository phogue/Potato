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
using Potato.Database.Shared.Builders;

namespace Potato.Database.Shared {
    /// <summary>
    /// Base class for serializing a collection of database objects
    /// to a usable compiled query
    /// </summary>
    public interface ISerializer {

        /// <summary>
        /// Compile a query, creating a new compiled object and assigning all reduced data
        /// from the serializer to the compiled query.
        /// </summary>
        /// <returns></returns>
        ICompiledQuery Compile();

        /// <summary>
        /// Converts a query into the required query (String by default), however
        /// the object may also be populated with additional requirements 
        /// </summary>
        /// <param name="method">The method object</param>
        /// <returns></returns>
        ISerializer Parse(IMethod method);

        /// <summary>
        /// Alias for Parse(Method method) without requirement of caller to convert type.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        ISerializer Parse(IDatabaseObject query);
    }
}
