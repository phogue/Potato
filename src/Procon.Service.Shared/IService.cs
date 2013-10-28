using System;
using System.Collections.Generic;

namespace Procon.Service.Shared {
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
    }
}
