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
        public String Vector { get; set; }
        public String Data { get; set; }
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
                using (MemoryStream memory = new MemoryStream()) {
                    using (CryptoStream crypto = new CryptoStream(memory, transform, CryptoStreamMode.Write)) {
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
            byte[] salt = new byte[8];

            using (RNGCryptoServiceProvider random = new RNGCryptoServiceProvider()) {
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
        protected byte[] DeriveKey(String password, byte[] salt) {
            byte[] key;

            using (var derived = new Rfc2898DeriveBytes(password, salt, 26000)) {
                key = derived.GetBytes(32);
            }

            return key;
        }

        public IConfigCommand Encrypt(String password) {
            // Hard error if no password is passed through, but the data is requested to be encrypted.
            if (password == null || password.Length <= 0) throw new ArgumentNullException("password");

            using (RijndaelManaged managed = new RijndaelManaged()) {
                // Generate a new salt.
                byte[] salt = this.GenerateSalt();
                this.Salt = Convert.ToBase64String(salt);

                // Generate new vector.
                managed.GenerateIV();
                this.Vector = Convert.ToBase64String(managed.IV);

                // Derive a key
                byte[] key = this.DeriveKey(password, salt);

                using (StringWriter writer = new StringWriter()) {
                    Potato.Core.Shared.Serialization.JsonSerialization.Minimal.Serialize(writer, this.Command);

                    byte[] text = Encoding.UTF8.GetBytes(writer.ToString());

                    using (ICryptoTransform transform = managed.CreateEncryptor(key, managed.IV)) {
                        this.Data = Convert.ToBase64String(this.CryptoTransform(text, transform));
                    }
                }
            }

            // Don't store the unencrypted data.
            this.Command = null;

            return this;
        }

        public IConfigCommand Decrypt(String password) {
            // Hard error if no password is passed through, but the data is requested to be encrypted.
            if (password == null || password.Length <= 0) throw new ArgumentNullException("password");

            if (this.Vector != null && this.Data != null) {
                // Decrypt to Command.
                using (RijndaelManaged managed = new RijndaelManaged()) {
                    // Fetch our salt
                    byte[] salt = Convert.FromBase64String(this.Salt);

                    // Fetch out vector
                    managed.IV = Convert.FromBase64String(this.Vector);

                    // Derive our key
                    byte[] key = this.DeriveKey(password, salt);

                    byte[] ciphertext = Convert.FromBase64String(this.Data);

                    using (ICryptoTransform transform = managed.CreateDecryptor(key, managed.IV)) {
                        String text = Encoding.UTF8.GetString(this.CryptoTransform(ciphertext, transform));

                        this.Command = Potato.Core.Shared.Serialization.JsonSerialization.Minimal.Deserialize<ICommand>(text);
                    }
                }
            }

            return this;
        }
    }
}
