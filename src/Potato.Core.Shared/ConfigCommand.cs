using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Potato.Net.Shared.Serialization;

namespace Potato.Core.Shared {
    /// <summary>
    /// A wrapper for a command being saved to a config
    /// </summary>
    public class ConfigCommand : IConfigCommand {
        public string Salt { get; set; }
        public string Vector { get; set; }
        public string Data { get; set; }
        public ICommand Command { get; set; }

        /// <summary>
        /// Transforms bytes using a given crypto transform.
        /// </summary>
        /// <param name="buffer">The data to encrypt</param>
        /// <param name="transform">The transform to use</param>
        /// <returns>The encrypted/decrypted bytes</returns>
        protected byte[] CryptoTransform(byte[] buffer, ICryptoTransform transform) {
            byte[] transformed;

            try {
                using (var memory = new MemoryStream()) {
                    using (var crypto = new CryptoStream(memory, transform, CryptoStreamMode.Write)) {
                        crypto.Write(buffer, 0, buffer.Length);
                    }

                    transformed = memory.ToArray();
                }
            }
            catch {
                transformed = new byte[0];
            }


            return transformed;
        }

        /// <summary>
        /// Generates random salt.
        /// </summary>
        /// <returns>Salted random, generated style.</returns>
        protected byte[] GenerateSalt() {
            var salt = new byte[8];

            using (var random = new RNGCryptoServiceProvider()) {
                random.GetBytes(salt);
            }

            return salt;
        }

        /// <summary>
        /// Derives a key from a password and some salt.
        /// </summary>
        /// <param name="password">The set password for to derive a key from</param>
        /// <param name="salt">A random salt</param>
        /// <returns>The key</returns>
        protected byte[] DeriveKey(string password, byte[] salt) {
            byte[] key;

            using (var derived = new Rfc2898DeriveBytes(password, salt, 26000)) {
                key = derived.GetBytes(32);
            }

            return key;
        }

        public IConfigCommand Encrypt(string password) {
            // Hard error if no password is passed through, but the data is requested to be encrypted.
            if (password == null || password.Length <= 0) throw new ArgumentNullException("password");

            using (var managed = new RijndaelManaged()) {
                // Generate a new salt.
                var salt = GenerateSalt();
                Salt = Convert.ToBase64String(salt);

                // Generate new vector.
                managed.GenerateIV();
                Vector = Convert.ToBase64String(managed.IV);

                // Derive a key
                var key = DeriveKey(password, salt);

                using (var writer = new StringWriter()) {
                    Potato.Core.Shared.Serialization.JsonSerialization.Minimal.Serialize(writer, Command);

                    var text = Encoding.UTF8.GetBytes(writer.ToString());

                    using (var transform = managed.CreateEncryptor(key, managed.IV)) {
                        Data = Convert.ToBase64String(CryptoTransform(text, transform));
                    }
                }
            }

            // Don't store the unencrypted data.
            Command = null;

            return this;
        }

        public IConfigCommand Decrypt(string password) {
            // Hard error if no password is passed through, but the data is requested to be encrypted.
            if (password == null || password.Length <= 0) throw new ArgumentNullException("password");

            if (Vector != null && Data != null) {
                // Decrypt to Command.
                using (var managed = new RijndaelManaged()) {
                    // Fetch our salt
                    var salt = Convert.FromBase64String(Salt);

                    // Fetch out vector
                    managed.IV = Convert.FromBase64String(Vector);

                    // Derive our key
                    var key = DeriveKey(password, salt);

                    var ciphertext = Convert.FromBase64String(Data);

                    using (var transform = managed.CreateDecryptor(key, managed.IV)) {
                        var text = Encoding.UTF8.GetString(CryptoTransform(ciphertext, transform));

                        Command = Potato.Core.Shared.Serialization.JsonSerialization.Minimal.Deserialize<ICommand>(text);
                    }
                }
            }

            return this;
        }
    }
}
