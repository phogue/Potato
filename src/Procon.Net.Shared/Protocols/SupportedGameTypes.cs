using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Procon.Net.Shared.Protocols {
    public static class SupportedGameTypes {

        /// <summary>
        /// List of cached supported game type attributes attached to their actual type.
        /// </summary>
        private static Dictionary<IProtocolType, Type> _supportedGames;

        /// <summary>
        /// Late loads Procon.Net.Protocols.*.dll's 
        /// </summary>
        private static IEnumerable<Assembly> LateBindGames() {
            List<Assembly> assemblies = new List<Assembly>() {
                Assembly.GetAssembly(typeof(IProtocol))
            };

            foreach (String protocol in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "Procon.Net.Protocols.*.dll", SearchOption.AllDirectories)) {
                try {
                    assemblies.Add(Assembly.LoadFile(protocol));
                }
                catch { }
            }

            return assemblies;
        }

        public static Dictionary<IProtocolType, Type> GetSupportedGames() {

            Dictionary<IProtocolType, Type> games = SupportedGameTypes._supportedGames;

            if (games == null) {

                // Load the supported games
                IEnumerable<Assembly> assemblies = SupportedGameTypes.LateBindGames();

                Regex supportedGamesNamespame = new Regex(@"^Procon\.Net\.Protocols.*");

                // Cache the results 
                SupportedGameTypes._supportedGames = games = (from gameClassType in assemblies.SelectMany(assembly => assembly.GetTypes())
                                                              let gameType = (gameClassType.GetCustomAttributes(typeof(IProtocolType), false) as IEnumerable<IProtocolType>).FirstOrDefault()
                                                where gameType != null &&
                                                      gameClassType != null &&
                                                      gameClassType.IsClass == true &&
                                                      gameClassType.IsAbstract == false &&
                                                      gameClassType.Namespace != null &&
                                                      supportedGamesNamespame.IsMatch(gameClassType.Namespace) == true &&
                                                      typeof(IProtocol).IsAssignableFrom(gameClassType)
                                                select new {
                                                    Name = gameType,
                                                    Type = gameClassType
                                                }).ToDictionary(w => new ProtocolType(w.Name) as IProtocolType, w => w.Type);
            }

            return games;
        }
    }
}
