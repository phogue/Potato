using System;

namespace Potato.Core.Shared {
    /// <summary>
    /// A wrapper for a command being saved to a config
    /// </summary>
    public interface IConfigCommand {
        /// <summary>
        /// Salt used to derive a key from a password
        /// </summary>
        string Salt { get; set; }

        /// <summary>
        /// A vector used for decrypting the data into a command.
        /// </summary>
        string Vector { get; set; }

        /// <summary>
        /// The encrypted data to be decrypted and deserialized into a command.
        /// </summary>
        string Data { get; set; }

        /// <summary>
        /// The plain command object, or the decrypted and deserialized command.
        /// </summary>
        ICommand Command { get; set; }

        /// <summary>
        /// Encrypts the command to the data
        /// </summary>
        /// <returns></returns>
        IConfigCommand Encrypt(string password);

        /// <summary>
        /// Decrypted the config command, if it's currently encrypted.
        /// </summary>
        /// <returns></returns>
        IConfigCommand Decrypt(string password);
    }
}
