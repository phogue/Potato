using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Procon.Net.Shared.Actions;

namespace Procon.Core.Shared.Models {
    [Serializable]
    public class GroupModel : CoreModel, IDisposable {
        /// <summary>
        /// The unique name of this group.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// All of the permissions each person has in this group.
        /// </summary>
        public List<PermissionModel> Permissions { get; set; }

        /// <summary>
        /// All of the accounts attached to this group.
        /// </summary>
        public List<AccountModel> Accounts { get; set; }

        public GroupModel() : base() {
            this.Name = String.Empty;
            this.Accounts = new List<AccountModel>();
            this.Permissions = new List<PermissionModel>();

            // Setup the default permissions.
            foreach (CommandType name in Enum.GetValues(typeof(CommandType)).Cast<CommandType>()) {
                if (name != CommandType.None) {
                    this.Permissions.Add(new PermissionModel() { CommandType = name });
                }
            }

            foreach (NetworkActionType name in Enum.GetValues(typeof(NetworkActionType)).Cast<NetworkActionType>()) {
                if (name != NetworkActionType.None) {
                    this.Permissions.Add(new PermissionModel() { Name = name.ToString() });
                }
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
