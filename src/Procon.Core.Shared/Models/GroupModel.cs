using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Procon.Net.Shared.Actions;

namespace Procon.Core.Shared.Models {
    /// <summary>
    /// A group attached to the security controller
    /// </summary>
    [Serializable]
    public class GroupModel : CoreModel, IDisposable {
        /// <summary>
        /// The unique name of this group.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Signifies the group as a guest group. Guest groups cannot have any 
        /// </summary>
        public bool IsGuest { get; set; }

        /// <summary>
        /// All of the permissions each person has in this group.
        /// </summary>
        public List<PermissionModel> Permissions { get; set; }

        /// <summary>
        /// All of the accounts attached to this group.
        /// </summary>
        public List<AccountModel> Accounts { get; set; }

        /// <summary>
        /// Initializes the group with default values.
        /// </summary>
        public GroupModel() : base() {
            this.Name = String.Empty;
            this.Accounts = new List<AccountModel>();
            this.Permissions = new List<PermissionModel>();

            // Setup the default permissions.
            foreach (CommandType name in Enum.GetValues(typeof(CommandType)).Cast<CommandType>().Where(name => name != CommandType.None)) {
                var permission = new PermissionModel() {
                    CommandType = name,
                    // All of the CommandType are boolean.
                    Traits = {
                        PermissionTraitsType.Boolean
                    }
                };

                var attributes = typeof(CommandType).GetMember(name.ToString()).First().GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attributes.Length > 0) {
                    permission.Description = attributes.Cast<DescriptionAttribute>().First().Description;
                }

                this.Permissions.Add(permission);
            }

            // List of permissions that are not simple boolean (they take into account the level of Authority)
            var authorityConstrainedActions = new List<String>() {
                "NetworkPlayerMove",
                "NetworkPlayerMoveForce",
                "NetworkPlayerMoveRotate",
                "NetworkPlayerMoveRotateForce",
                "NetworkPlayerBan",
                "NetworkPlayerUnban",
                "NetworkPlayerKick",
                "NetworkPlayerKill"
            };

            foreach (NetworkActionType name in Enum.GetValues(typeof(NetworkActionType)).Cast<NetworkActionType>().Where(name => name != NetworkActionType.None)) {
                var permission = new PermissionModel() {
                    Name = name.ToString()
                };

                if (authorityConstrainedActions.Contains(name.ToString()) == false) {
                    permission.Traits.Add(PermissionTraitsType.Boolean);
                }

                var attributes = typeof(NetworkActionType).GetMember(name.ToString()).First().GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attributes.Length > 0) {
                    permission.Description = attributes.Cast<DescriptionAttribute>().First().Description;
                }

                this.Permissions.Add(permission);
            }

            List<String> booleanTraits = new List<String>() { PermissionTraitsType.Boolean };

            // Set default boolean traits for default permissions
            Dictionary<String, List<String>> defaultTraits = new Dictionary<String, List<String>>() {
                { CommandType.SecurityGroupAddAccount.ToString(), booleanTraits },
                { CommandType.SecurityRemoveAccount.ToString(), booleanTraits },
                { CommandType.SecurityAccountAuthenticate.ToString(), booleanTraits },
                { CommandType.SecurityAccountSetPassword.ToString(), booleanTraits },
                { CommandType.SecurityAccountSetPasswordHash.ToString(), booleanTraits },
                { CommandType.SecurityAccountSetPreferredLanguageCode.ToString(), booleanTraits }

                // @todo more
            };

            foreach (var trait in defaultTraits) {
                this.Permissions.First(permission => permission.Name == trait.Key).Traits.AddRange(trait.Value);
            }
        }

        public void Dispose() {
            foreach (AccountModel account in this.Accounts) {
                account.Dispose();
            }

            foreach (PermissionModel permission in this.Permissions) {
                permission.Dispose();
            }

            this.Name = null;

            this.Accounts.Clear();
            this.Accounts = null;

            this.Permissions.Clear();
            this.Permissions = null;
        }
    }
}
