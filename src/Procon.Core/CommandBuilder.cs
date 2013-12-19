using System;
using System.Collections.Generic;
using Procon.Database.Serialization;

namespace Procon.Core {
    /// <summary>
    /// Helps shortcut some of the command code by building and returning a command.
    /// </summary>
    /// <remarks>
    ///     <para>This class will be added to as I go, just to cleanup some of the existing code.</para>
    /// </remarks>
    public static class CommandBuilder {

        /// <summary>
        /// Builds a command to send a DatabaseQuery with the database group to use.
        /// </summary>
        /// <param name="group">The name of the database group to use</param>
        /// <param name="queries">The queries to send </param>
        /// <returns>The build command to dispatch</returns>
        public static Command DatabaseQuery(String group, params IDatabaseObject[] queries) {
            return new Command() {
                CommandType = CommandType.DatabaseQuery,
                Parameters = new List<CommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<String>() {
                                group
                            }
                        }
                    },
                    new CommandParameter() {
                        Data = {
                            Queries = new List<IDatabaseObject>(queries)
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Builds a command to send a DatabaseQuery.
        /// </summary>
        /// <param name="queries">The queries to send </param>
        /// <returns>The build command to dispatch</returns>
        public static Command DatabaseQuery(params IDatabaseObject[] queries) {
            return new Command() {
                CommandType = CommandType.DatabaseQuery,
                Parameters = new List<CommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Queries = new List<IDatabaseObject>(queries)
                        }
                    }
                }
            };
        }
    }
}
