#region Copyright
// Copyright 2015 Geoff Green.
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
using Newtonsoft.Json;

namespace Potato.Core.Shared.Events {
    /// <summary>
    /// Defines a consistent structure that events should be logged or serialized with.
    /// </summary>
    /// <remarks>
    ///     <pre>This is done in a very non-clever way deliberately. Think of it as an Interface that should never change, but instead be added to.</pre>
    /// </remarks>
    [Serializable]
    public sealed class GenericEvent : CommandResult, IGenericEvent {

        public ulong Id { get; set; }

        public string Name { get; set; }

        [JsonIgnore]
        public GenericEventType GenericEventType {
            get { return _eventType; }
            set {
                _eventType = value;

                if (_eventType != GenericEventType.None) {
                    Name = value.ToString();
                }
            }
        }
        private GenericEventType _eventType;

        /// <summary>
        /// Converts a command result into a new more specialized generic event.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="eventType"></param>
        /// <returns></returns>
        public static GenericEvent ConvertToGenericEvent(ICommandResult result, GenericEventType eventType) {
            return new GenericEvent() {
                GenericEventType = eventType,
                Message = result.Message,
                Stamp = result.Stamp,
                Scope = result.Scope,
                Then = result.Then,
                Now = result.Now
            };
        }
    }
}
