#region

using System;
using System.Collections.Generic;
using System.Linq;
using Procon.Core.Shared;

#endregion

namespace Procon.Core.Test {
    public class TestHelpers {
        /// <summary>
        ///     Converts a list of object parameters into a list of CommandParameters with a single value in
        ///     the Content attribute.
        /// </summary>
        /// <remarks>This is implemented to convert from the old method of parsing parameters to the new.</remarks>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static List<CommandParameter> ObjectListToContentList(List<Object> parameters) {
            return parameters.Select(param => new CommandParameter() {
                Data = {
                    Content = new List<string>() {
                        param.ToString()
                    }
                }
            }).ToList();
        }
    }
}