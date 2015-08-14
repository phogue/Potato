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
using Potato.Core.Events;
using Potato.Core.Localization;
using Potato.Core.Security;
using Potato.Core.Variables;

namespace Potato.Core {
    /// <summary>
    /// Holds static references to common functionality across Potato.
    /// </summary>
    /// <remarks>This is kind-of, sort-of global variables but it's restricted to Potato's AppDomain only.</remarks>
    public class SharedReferences {
        /// <summary>
        /// Master list of variables.
        /// </summary>
        private static VariableController _masterVariables;

        /// <summary>
        /// Full list of all available languages.
        /// </summary>
        private static LanguageController _masterLanguages;

        /// <summary>
        /// The main security controller
        /// </summary>
        private static SecurityController _masterSecurity;

        /// <summary>
        /// The main events logging and handling
        /// </summary>
        private static EventsController _masterEvents;

        /// <summary>
        /// Stores a reference to the static variables controller by default, but can
        /// be overridden for unit testing.
        /// </summary>
        public VariableController Variables { get; set; }

        /// <summary>
        /// Stores a reference to the static language controller by default, but can
        /// be overridden for unit testing.
        /// </summary>
        public LanguageController Languages { get; set; }

        /// <summary>
        /// Stores a reference to the static security controller by default, but can
        /// be overridden for unit testing.
        /// </summary>
        public SecurityController Security { get; set; }

        /// <summary>
        /// Stores a reference to the static events controller by default, but can
        /// be overridden for unit testing.
        /// </summary>
        public EventsController Events { get; set; }

        /// <summary>
        /// Initializes the default references from the static controllers.
        /// </summary>
        public SharedReferences() : base() {
            Variables = _masterVariables;
            Languages = _masterLanguages;
            Security = _masterSecurity;
            Events = _masterEvents;
        }

        static SharedReferences() {
            Setup();
        }

        /// <summary>
        /// Sets up the static controllers in this class to all use one-another for their internal
        /// references to the objects. This method is used so once all the static controllers have been setup
        /// we can update all of their references in one hit, ensuring they all communicate with each other.
        /// 
        /// This makes the default controllers coupled with one another, but not a particular reference of the
        /// object (useful for unit testing!). The alternative is to not have the static objects at all, but the
        /// code does get very messy on some of the controllers that go a few levels deep (Potato.Core.Security or ..Plugins)
        /// </summary>
        public static void Setup() {
            _masterVariables = new VariableController();
            _masterLanguages = new LanguageController();
            _masterSecurity = new SecurityController();
            _masterEvents = new EventsController();

            if (_masterVariables != null && _masterLanguages != null && _masterSecurity != null && _masterEvents != null) {
                _masterVariables.Shared.Variables = _masterLanguages.Shared.Variables = _masterSecurity.Shared.Variables = _masterEvents.Shared.Variables = _masterVariables;
                _masterVariables.Shared.Languages = _masterLanguages.Shared.Languages = _masterSecurity.Shared.Languages = _masterEvents.Shared.Languages = _masterLanguages;
                _masterVariables.Shared.Security = _masterLanguages.Shared.Security = _masterSecurity.Shared.Security = _masterEvents.Shared.Security = _masterSecurity;
                _masterVariables.Shared.Events = _masterLanguages.Shared.Events = _masterSecurity.Shared.Events = _masterEvents.Shared.Events = _masterEvents;

                _masterVariables.Execute();
                _masterLanguages.Execute();
                _masterSecurity.Execute();
                _masterEvents.Execute();
            }
        }
    }
}
