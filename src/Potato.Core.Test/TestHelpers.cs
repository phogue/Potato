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
#region

using System;
using System.Collections.Generic;
using System.Linq;
using Potato.Core.Shared;

#endregion

namespace Potato.Core.Test {
    public class TestHelpers {
        /// <summary>
        ///     Converts a list of object parameters into a list of CommandParameters with a single value in
        ///     the Content attribute.
        /// </summary>
        /// <remarks>This is implemented to convert from the old method of parsing parameters to the new.</remarks>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static List<ICommandParameter> ObjectListToContentList(List<Object> parameters) {
            return parameters.Select(param => new CommandParameter() {
                Data = {
                    Content = new List<string>() {
                        param.ToString()
                    }
                }
            }).Cast<ICommandParameter>().ToList();
        }
    }
}