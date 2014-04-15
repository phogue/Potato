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
namespace Potato.Fuzzy.Tokens.Object {
    /// <summary>
    /// A reference to an object and how to interact with it.
    /// </summary>
    public interface IThingReference {

        /// <summary>
        /// Tests compatability with another thing reference, making sure both
        /// are the same type or can be used together.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        bool CompatibleWith(IThingReference other);

        /// <summary>
        /// Combines two thing references, used so keeping track of the references can be merged
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        IThingReference Union(IThingReference other);

        /// <summary>
        /// Returns only references in this object and not in the other reference
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        IThingReference Complement(IThingReference other);
    }
}
