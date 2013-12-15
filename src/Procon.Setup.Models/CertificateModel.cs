﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using Procon.Service.Shared;
using X509Certificate = Org.BouncyCastle.X509.X509Certificate;

namespace Procon.Setup.Models {
    public class CertificateModel : INotifyPropertyChanged {
        private FileSystemWatcher _watcher;
        private string _password;
        private bool _exists;

        /// <summary>
        /// The password for the certificate
        /// </summary>
        public String Password {
            get { return _password; }
            set {
                if (_password != value) {
                    _password = value;
                    this.OnPropertyChanged("Password");
                }
            }
        }

        /// <summary>
        /// Check if the certificate exists or not.
        /// </summary>
        public bool Exists {
            get { return _exists; }
            set {
                if (_exists != value) {
                    _exists = value;
                    this.OnPropertyChanged("Exists");
                }
            }
        }

        /// <summary>
        /// Sets up the directory watch to look for changes in the certificates folder
        /// </summary>
        public CertificateModel() {
            this.RandomizePassword();

            this.Watch();
        }

        protected void Watch() {
            // Create the directory if it does not exist.
            Directory.CreateDirectory(Defines.CertificatesDirectory);

            this._watcher = new FileSystemWatcher(Defines.CertificatesDirectory) {
                NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName,
                Filter = "*.pfx"
            };

            this._watcher.Created += WatcherOnChanged;
            this._watcher.Deleted += WatcherOnChanged;
            this._watcher.Changed += WatcherOnChanged;
            this._watcher.Renamed += WatcherOnChanged;

            this._watcher.EnableRaisingEvents = true;

            this.Exists = File.Exists(Defines.CertificatesDirectoryCommandServerPfx);
        }

        private void WatcherOnChanged(object sender, FileSystemEventArgs fileSystemEventArgs) {
            this.Exists = File.Exists(Defines.CertificatesDirectoryCommandServerPfx);
        }

        /// <summary>
        /// Randomizes the password set in this certificate.
        /// </summary>
        public void RandomizePassword() {
            const string characters = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
            String password = "";

            SecureRandom random = new SecureRandom(new CryptoApiRandomGenerator());

            while (password.Length < 15) {
                password += characters[random.Next(characters.Length)];
            }

            this.Password = password;
        }

        /// <summary>
        /// Generates and saves a new certificate to the default CommandServer plx path
        /// </summary>
        public void Generate() {
            RsaKeyPairGenerator rsaKeyPairGenerator = new RsaKeyPairGenerator();
            rsaKeyPairGenerator.Init(new KeyGenerationParameters(new SecureRandom(new CryptoApiRandomGenerator()), 2048));

            AsymmetricCipherKeyPair asymmetricCipherKeyPair = rsaKeyPairGenerator.GenerateKeyPair();

            X509Name certificateName = new X509Name("CN=" + Environment.MachineName);
            BigInteger serialNumber = BigInteger.ProbablePrime(120, new SecureRandom());

            X509V3CertificateGenerator certificateGenerator = new X509V3CertificateGenerator();
            certificateGenerator.SetSerialNumber(serialNumber);
            certificateGenerator.SetSubjectDN(certificateName);
            certificateGenerator.SetIssuerDN(certificateName);
            certificateGenerator.SetNotAfter(DateTime.Now.AddYears(1));
            certificateGenerator.SetNotBefore(DateTime.Now.Subtract(new TimeSpan(7, 0, 0, 0)));
            certificateGenerator.SetSignatureAlgorithm("Sha1WithRSA");
            certificateGenerator.SetPublicKey(asymmetricCipherKeyPair.Public);

            certificateGenerator.AddExtension(
                X509Extensions.AuthorityKeyIdentifier.Id,
                false,
                new AuthorityKeyIdentifier(
                    SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(asymmetricCipherKeyPair.Public),
                    new GeneralNames(new GeneralName(certificateName)),
                    serialNumber
                )
            );

            certificateGenerator.AddExtension(
                X509Extensions.ExtendedKeyUsage.Id,
                false,
                new ExtendedKeyUsage(KeyPurposeID.IdKPServerAuth)
            );

            X509CertificateEntry certificateEntry = new X509CertificateEntry(certificateGenerator.Generate(asymmetricCipherKeyPair.Private));

            Pkcs12Store store = new Pkcs12Store();
            store.SetCertificateEntry(certificateName.ToString(), certificateEntry);
            store.SetKeyEntry(certificateName.ToString(), new AsymmetricKeyEntry(asymmetricCipherKeyPair.Private), new[] {
                certificateEntry
            });

            // Save to the file system
            using (var filestream = new FileStream(Defines.CertificatesDirectoryCommandServerPfx, FileMode.Create, FileAccess.ReadWrite)) {
                store.Save(filestream, this.Password.ToCharArray(), new SecureRandom());
            }
        }

        /// <summary>
        /// Deletes the existing certificate
        /// </summary>
        /// <returns>True if the file no longer exists, false if an error occured</returns>
        public bool Delete() {
            bool deleted = true;
            if (File.Exists(Defines.CertificatesDirectoryCommandServerPfx)) {
                try {
                    File.Delete(Defines.CertificatesDirectoryCommandServerPfx);
                }
                catch {
                    deleted = false;
                }
            }

            return deleted;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}