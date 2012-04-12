// Copyright 2011 Geoffrey 'Phogue' Green
// 
// http://www.phogue.net
//  
// This file is part of Procon 2.
// 
// Procon 2 is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Procon 2 is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Procon 2.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Procon.Core.Interfaces.Variables {
    using Procon.Net.Utils;

    public interface IVariableController {

        /// <summary>
        /// Anything in this list is volatile and will not be saved on
        /// exit.
        /// </summary>
        List<Variable> Variables { get; }

        /// <summary>
        /// This will first set the value, then set the value in the archived list
        /// which will be saved to the config
        /// </summary>
        /// <param name="initiator">Details of the commands origin</param>
        /// <param name="key">The unique key of the variable to set</param>
        /// <param name="value">The value of the variable</param>
        /// <returns></returns>
        Object Set(CommandInitiator initiator, string key, Object value);

        /// <summary>
        /// This will first set the value, then set the value in the archived list
        /// which will be saved to the config
        /// </summary>
        /// <param name="initiator">Details of the commands origin</param>
        /// <param name="key">The unique key of the variable to set</param>
        /// <param name="value">The value of the variable</param>
        /// <returns></returns>
        Object SetA(CommandInitiator initiator, string key, Object value);

        /// <summary>
        /// Gets and converts a value given a key
        /// </summary>
        /// <typeparam name="T">The type of value to return</typeparam>
        /// <param name="initiator">Details of the commands origin</param>
        /// <param name="key">The unique key of the variable to fetch</param>
        /// <returns>The converted value of the variable with the specified key</returns>
        T Get<T>(CommandInitiator initiator, string key, T defaultValue = default(T));

        /// <summary>
        /// Gets a raw value given a key, returned as a Object
        /// </summary>
        /// <param name="initiator">Details of the commands origin</param>
        /// <param name="key">The unique key of the variable to fetch</param>
        /// <returns>The raw object with no conversion</returns>
        Object Get(CommandInitiator initiator, string key, Object defaultValue = null);

        #region CommonVariableNames helper methods

        /// <summary>
        /// This will first set the value, then set the value in the archived list
        /// which will be saved to the config
        /// </summary>
        /// <param name="initiator">Details of the commands origin</param>
        /// <param name="key">The unique key of the variable to set</param>
        /// <param name="value">The value of the variable</param>
        /// <returns></returns>
        Object Set(CommandInitiator initiator, CommonVariableNames key, Object value);

        /// <summary>
        /// This will first set the value, then set the value in the archived list
        /// which will be saved to the config
        /// </summary>
        /// <param name="initiator">Details of the commands origin</param>
        /// <param name="key">The unique key of the variable to set</param>
        /// <param name="value">The value of the variable</param>
        /// <returns></returns>
        Object SetA(CommandInitiator initiator, CommonVariableNames key, Object value);

        /// <summary>
        /// Gets and converts a value given a key
        /// </summary>
        /// <typeparam name="T">The type of value to return</typeparam>
        /// <param name="initiator">Details of the commands origin</param>
        /// <param name="key">The unique key of the variable to fetch</param>
        /// <returns>The converted value of the variable with the specified key</returns>
        T Get<T>(CommandInitiator initiator, CommonVariableNames key, T defaultValue = default(T));

        /// <summary>
        /// Gets a raw value given a key, returned as a Object
        /// </summary>
        /// <param name="initiator">Details of the commands origin</param>
        /// <param name="key">The unique key of the variable to fetch</param>
        /// <returns>The raw object with no conversion</returns>
        Object Get(CommandInitiator initiator, CommonVariableNames key, Object defaultValue = null);

        #endregion
    }
}
