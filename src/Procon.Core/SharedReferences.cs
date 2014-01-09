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
        private readonly static VariableController MasterVariables;

        /// <summary>
        /// Full list of all available languages.
        /// </summary>
        private readonly static LanguageController MasterLanguages;

        /// <summary>
        /// The main security controller
        /// </summary>
        private readonly static SecurityController MasterSecurity;

        /// <summary>
        /// The main events logging and handling
        /// </summary>
        private readonly static EventsController MasterEvents;

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
            this.Variables = SharedReferences.MasterVariables;
            this.Languages = SharedReferences.MasterLanguages;
            this.Security = SharedReferences.MasterSecurity;
            this.Events = SharedReferences.MasterEvents;
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
        static SharedReferences() {

            MasterVariables = new VariableController();
            MasterLanguages = new LanguageController();
            MasterSecurity = new SecurityController();
            MasterEvents = new EventsController();

            if (SharedReferences.MasterVariables != null && SharedReferences.MasterLanguages != null && SharedReferences.MasterSecurity != null && SharedReferences.MasterEvents != null) {
                SharedReferences.MasterVariables.Shared.Variables = SharedReferences.MasterLanguages.Shared.Variables = SharedReferences.MasterSecurity.Shared.Variables = SharedReferences.MasterEvents.Shared.Variables = SharedReferences.MasterVariables;
                SharedReferences.MasterVariables.Shared.Languages = SharedReferences.MasterLanguages.Shared.Languages = SharedReferences.MasterSecurity.Shared.Languages = SharedReferences.MasterEvents.Shared.Languages = SharedReferences.MasterLanguages;
                SharedReferences.MasterVariables.Shared.Security = SharedReferences.MasterLanguages.Shared.Security = SharedReferences.MasterSecurity.Shared.Security = SharedReferences.MasterEvents.Shared.Security = SharedReferences.MasterSecurity;
                SharedReferences.MasterVariables.Shared.Events = SharedReferences.MasterLanguages.Shared.Events = SharedReferences.MasterSecurity.Shared.Events = SharedReferences.MasterEvents.Shared.Events = SharedReferences.MasterEvents;

                MasterVariables.Execute();
                MasterLanguages.Execute();
                MasterSecurity.Execute();
                MasterEvents.Execute();
            }
        }
    }
}
