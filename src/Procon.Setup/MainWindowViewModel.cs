using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using MahApps.Metro.Controls;
using Procon.Core.Shared;
using Procon.Core.Shared.Models;
using ICommand = System.Windows.Input.ICommand;

namespace Procon.Setup {
    using Procon.Core;
    using Procon.Setup.Models;
    public class MainWindowViewModel : INotifyPropertyChanged  {
        
        /// <summary>
        /// The Procon instance for this directory.
        /// </summary>
        protected InstanceController Instance { get; set; }

        /// <summary>
        /// List of cloned languages from Procon.
        /// </summary>
        protected ObservableCollection<LanguageModel> LanguageModels { get; set; }

        /// <summary>
        /// List of panorama groups for the language tab
        /// </summary>
        public ObservableCollection<PanoramaGroup> LanguageGroups { get; set; }

        /// <summary>
        /// THe loaded or generated certificate
        /// </summary>
        public CertificateModel Certificate { get; set; }

        /// <summary>
        /// General bool to describe the application is currently doing something or not.
        /// </summary>
        public bool Busy {
            get { return _busy; }
            set {
                if (_busy != value) {
                    _busy = value;

                    OnPropertyChanged("Busy");
                }
            }
        }
        private bool _busy;

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        /// <summary>
        /// Select a language with a language-code parameter. 
        /// </summary>
        public ICommand SelectLanguageCommand { get; set; }

        public ICommand ButtonCommand { get; set; }

        /// <summary>
        /// Randomize the certificate password
        /// </summary>
        public ICommand CertificateRandomizePasswordCommand { get; set; }

        /// <summary>
        /// Generate a new certificate with the supplied password
        /// </summary>
        public ICommand CertificateGenerateCommand { get; set; }

        /// <summary>
        /// Delete the existing certificate.
        /// </summary>
        public ICommand CertificateDeleteCommand { get; set; }

        public MainWindowViewModel() {
            
            SelectLanguageCommand = new RelayCommand(new Action<Object>(SelectLanguage));
            ButtonCommand = new RelayCommand(new Action<Object>(ShowMessage));
            CertificateRandomizePasswordCommand = new RelayCommand(new Action<Object>(CertificateRandomizePassword));
            CertificateGenerateCommand = new RelayCommand(new Action<Object>(CertificateGenerate));
            CertificateDeleteCommand = new RelayCommand(new Action<Object>(CertificateDelete));

            this.LanguageModels = new ObservableCollection<LanguageModel>();

            this.LanguageGroups = new ObservableCollection<PanoramaGroup>() {
                new PanoramaGroup("languages", this.LanguageModels)
            };

            this.Certificate = new CertificateModel();
        }

        /// <summary>
        /// Loads and assigns events from a Procon instance.
        /// </summary>
        public bool CreateInstance() {
            bool created = false;

            this.Instance = new InstanceController().Execute() as InstanceController;

            if (this.Instance != null) {
                this.Instance.Shared.Variables.Variable(CommonVariableNames.LocalizationDefaultLanguageCode).PropertyChanged += new PropertyChangedEventHandler(LocalizationDefaultLanguageCode_PropertyChanged);

                created = true;
            }

            return created;
        }

        /// <summary>
        /// Fired whenever the CommonVariableNames.LocalizationDefaultLanguageCode variable is edited.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LocalizationDefaultLanguageCode_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            this.RefreshSelectedLanguage();
        }

        /// <summary>
        /// Refreshes from the data in the instance of Procon (non observable lists)
        /// </summary>
        public void RefreshInstance() {
            this.RefreshLanguages();
        }

        /// <summary>
        /// Refreshes all of the languages from the instance.
        /// </summary>
        protected void RefreshLanguages() {
            this.LanguageModels.Clear();

            foreach (Procon.Core.Localization.LanguageConfig language in this.Instance.Shared.Languages.LoadedLanguageFiles) {
                this.LanguageModels.Add(new LanguageModel() {
                    CountryCode = language.LanguageModel.CountryCode,
                    LanguageCode = language.LanguageModel.LanguageCode,
                    Name = language.LanguageModel.NativeName
                });
            }

            this.RefreshSelectedLanguage();
        }

        /// <summary>
        /// Grabs the selected language variable and sets our local "IsSelected" boolean accordingly.
        /// </summary>
        protected void RefreshSelectedLanguage() {
            if (this.Instance != null) {
                String localizationDefaultLanguageCode = this.Instance.Shared.Variables.Get(CommonVariableNames.LocalizationDefaultLanguageCode, "en-UK");

                foreach (LanguageModel language in this.LanguageModels) {
                    language.IsSelected = language.LanguageCode == localizationDefaultLanguageCode;
                }
            }
        }

        /// <summary>
        /// Fired when a user selects another language
        /// </summary>
        /// <param name="value">The value is the language code of the selected language</param>
        public void SelectLanguage(Object value) {
            if (this.Instance != null) {
                this.Instance.Shared.Variables.Tunnel(CommandBuilder.VariablesSetA(CommonVariableNames.LocalizationDefaultLanguageCode, value.ToString()).SetOrigin(CommandOrigin.Local));
            }
        }

        public void CertificateRandomizePassword(Object value) {
            this.Certificate.RandomizePassword();
        }

        public void CertificateGenerate(Object value) {
            this.Busy = true;

            Task.Factory.StartNew(() => {
                this.Certificate.Generate();

                this.Busy = false;
            });
        }

        public void CertificateDelete(Object value) {
            this.Busy = true;

            Task.Factory.StartNew(() => {
                this.Certificate.Delete();

                this.Busy = false;
            });
        }

        public void ShowMessage(Object value) {

            for (var x = 0; x < 20; x++) {
                this.LanguageModels.Add(new LanguageModel() {
                    CountryCode = "GB",
                    LanguageCode = "en-UK"
                });

                this.LanguageModels.Add(new LanguageModel() {
                    CountryCode = "DE",
                    LanguageCode = "de-DE"
                });
            }
        }
    }
}
