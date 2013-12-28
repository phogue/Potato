using System;
using System.ComponentModel;

namespace Procon.Setup.Models {
    /// <summary>
    /// A loaded language file within Procon.
    /// </summary>
    public class LanguageModel : INotifyPropertyChanged {
        private bool _isSelected;
        private string _name;
        private string _countryCode;
        private string _languageCode;

        /// <summary>
        /// The IETF language tag
        /// </summary>
        public String LanguageCode {
            get { return _languageCode; }
            set {
                if (_languageCode != value) {
                    _languageCode = value;
                    this.OnPropertyChanged("LanguageCode");
                }
            }
        }

        /// <summary>
        /// The main location for this language, or country of origin.
        /// </summary>
        public String CountryCode {
            get { return _countryCode; }
            set {
                if (_countryCode != value) {
                    _countryCode = value;
                    this.OnPropertyChanged("CountryCode");
                }
            }
        }

        /// <summary>
        /// The name of the language
        /// </summary>
        public String Name {
            get { return _name; }
            set {
                if (_name != value) {
                    _name = value;
                    this.OnPropertyChanged("Name");
                }
            }
        }

        /// <summary>
        /// If this is currently selected default language
        /// </summary>
        public bool IsSelected {
            get { return _isSelected; }
            set {
                if (_isSelected != value) {
                    _isSelected = value;
                    this.OnPropertyChanged("IsSelected");
                }
            }
        }

        /// <summary>
        /// Fired whenever a property is changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            } 
        }
    }
}
