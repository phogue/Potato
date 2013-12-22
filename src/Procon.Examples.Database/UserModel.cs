using System;
using Procon.Core.Shared.Database;

namespace Procon.Examples.Database {
    public class UserModel : DatabaseModel<UserModel> {
        /// <summary>
        /// The name of the user
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// The users age.
        /// </summary>
        public int Age { get; set; }
    }
}
