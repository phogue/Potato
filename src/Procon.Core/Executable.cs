using System.IO;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Procon.Core.Events;
using Procon.Core.Localization;
using Procon.Core.Security;
using Procon.Core.Variables;
using Procon.Service.Shared;

namespace Procon.Core {
    /// <summary>
    /// Holds static references to common functionality across procon.
    /// </summary>
    /// <remarks>This is kind-of, sort-of global variables but it's restricted to Procon's AppDomain only.</remarks>
    public abstract class Executable : ExecutableBase {

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
        [XmlIgnore, JsonIgnore]
        public VariableController Variables { get; set; }

        /// <summary>
        /// Stores a reference to the static language controller by default, but can
        /// be overridden for unit testing.
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public LanguageController Languages { get; set; }

        /// <summary>
        /// Stores a reference to the static security controller by default, but can
        /// be overridden for unit testing.
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public SecurityController Security { get; set; }

        /// <summary>
        /// Stores a reference to the static events controller by default, but can
        /// be overridden for unit testing.
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public EventsController Events { get; set; }

        protected Executable() : base() {
            this.Variables = Executable.MasterVariables;
            this.Languages = Executable.MasterLanguages;
            this.Security = Executable.MasterSecurity;
            this.Events = Executable.MasterEvents;
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
        static Executable() {

            MasterVariables = new VariableController().Execute() as VariableController;
            MasterLanguages = new LanguageController().Execute() as LanguageController;
            MasterSecurity = new SecurityController().Execute() as SecurityController;
            MasterEvents = new EventsController().Execute() as EventsController;

            if (Executable.MasterVariables != null && Executable.MasterLanguages != null && Executable.MasterSecurity != null && Executable.MasterEvents != null) {
                Executable.MasterVariables.Variables = Executable.MasterLanguages.Variables = Executable.MasterSecurity.Variables = Executable.MasterEvents.Variables = Executable.MasterVariables;
                Executable.MasterVariables.Languages = Executable.MasterLanguages.Languages = Executable.MasterSecurity.Languages = Executable.MasterEvents.Languages = Executable.MasterLanguages;
                Executable.MasterVariables.Security = Executable.MasterLanguages.Security = Executable.MasterSecurity.Security = Executable.MasterEvents.Security = Executable.MasterSecurity;
                Executable.MasterVariables.Events = Executable.MasterLanguages.Events = Executable.MasterSecurity.Events = Executable.MasterEvents.Events = Executable.MasterEvents;

                Executable.MasterSecurity.Groups.ForEach(group => {
                    group.Variables = Executable.MasterVariables;
                    group.Languages = Executable.MasterLanguages;
                    group.Security = Executable.MasterSecurity;
                    group.Events = Executable.MasterEvents;

                    group.Accounts.ForEach(account => {
                        account.Variables = Executable.MasterVariables;
                        account.Languages = Executable.MasterLanguages;
                        account.Security = Executable.MasterSecurity;
                        account.Events = Executable.MasterEvents;
                    });
                });
            }
        }

        /// <summary>
        /// Loads the configuration file.
        /// </summary>
        /// <returns></returns>
        public override ExecutableBase Execute() {
            this.Execute(new Command() {
                Origin = CommandOrigin.Local
            }, new Config().Load(new DirectoryInfo(Defines.ConfigsDirectory)));

            return this;
        }
    }
}
