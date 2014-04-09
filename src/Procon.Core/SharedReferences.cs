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
using Procon.Core.Events;
using Procon.Core.Localization;
using Procon.Core.Security;
using Procon.Core.Variables;

namespace Procon.Core {
    /// <summary>
    /// Holds static references to common functionality across procon.
    /// </summary>
    /// <remarks>This is kind-of, sort-of global variables but it's restricted to Procon's AppDomain only.</remarks>
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
            this.Variables = SharedReferences._masterVariables;
            this.Languages = SharedReferences._masterLanguages;
            this.Security = SharedReferences._masterSecurity;
            this.Events = SharedReferences._masterEvents;
        }

        static SharedReferences() {
            SharedReferences.Setup();
        }

        /// <summary>
        /// Sets up the static controllers in this class to all use one-another for their internal
        /// references to the objects. This method is used so once all the static controllers have been setup
        /// we can update all of their references in one hit, ensuring they all communicate with each other.
        /// 
        /// This makes the default controllers coupled with one another, but not a particular reference of the
        /// object (useful for unit testing!). The alternative is to not have the static objects at all, but the
        /// code does get very messy on some of the controllers that go a few levels deep (Procon.Core.Security or ..Plugins)
        /// </summary>
        public static void Setup() {
            SharedReferences._masterVariables = new VariableController();
            SharedReferences._masterLanguages = new LanguageController();
            SharedReferences._masterSecurity = new SecurityController();
            SharedReferences._masterEvents = new EventsController();

            if (SharedReferences._masterVariables != null && SharedReferences._masterLanguages != null && SharedReferences._masterSecurity != null && SharedReferences._masterEvents != null) {
                SharedReferences._masterVariables.Shared.Variables = SharedReferences._masterLanguages.Shared.Variables = SharedReferences._masterSecurity.Shared.Variables = SharedReferences._masterEvents.Shared.Variables = SharedReferences._masterVariables;
                SharedReferences._masterVariables.Shared.Languages = SharedReferences._masterLanguages.Shared.Languages = SharedReferences._masterSecurity.Shared.Languages = SharedReferences._masterEvents.Shared.Languages = SharedReferences._masterLanguages;
                SharedReferences._masterVariables.Shared.Security = SharedReferences._masterLanguages.Shared.Security = SharedReferences._masterSecurity.Shared.Security = SharedReferences._masterEvents.Shared.Security = SharedReferences._masterSecurity;
                SharedReferences._masterVariables.Shared.Events = SharedReferences._masterLanguages.Shared.Events = SharedReferences._masterSecurity.Shared.Events = SharedReferences._masterEvents.Shared.Events = SharedReferences._masterEvents;

                _masterVariables.Execute();
                _masterLanguages.Execute();
                _masterSecurity.Execute();
                _masterEvents.Execute();
            }
        }
    }
}
