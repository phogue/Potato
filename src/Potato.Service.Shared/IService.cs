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

namespace Potato.Service.Shared {
    /// <summary>
    /// Interface for communication between the service and Potato appdomain.
    /// </summary>
    public interface IService {
        /// <summary>
        /// A method to signify the instance should begin, loading it's configs and setting up connections & what not.
        /// </summary>
        void Start();

        /// <summary>
        /// Tells the instance the configs should now be written.
        /// </summary>
        void WriteConfig();

        /// <summary>
        /// Dispose of this instance. The AppDomain will be clonsed shortly afterwards.
        /// </summary>
        void Dispose();

        /// <summary>
        /// Parse the command line args, putting them into the Variables object for the instance.
        /// </summary>
        /// <param name="arguments"></param>
        void ParseCommandLineArguments(List<String> arguments);

        /// <summary>
        /// Polls the service for a running status or other message.
        /// </summary>
        ServiceMessage PollService();

        /// <summary>
        /// Requests the service execute a message, returning the result as a message.
        /// </summary>
        /// <remarks>This will only ever handle very simple requests that require strings/integers for arguments</remarks>
        /// <param name="message">The message to execute</param>
        /// <returns>The result of the message</returns>
        ServiceMessage ExecuteMessage(ServiceMessage message);
    }
}
