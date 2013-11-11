using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Procon.Setup.Properties;

namespace Procon.Setup.Models {
    public class LanguageModel : INotifyPropertyChanged {
        private bool _isSelected;
        private string _name;
        private string _countryCode;
        private string _languageCode;

        public String LanguageCode {
            get { return _languageCode; }
            set {
                if (_languageCode != value) {
                    _languageCode = value;
                    this.OnPropertyChanged("LanguageCode");
                }
            }
        }

        public String CountryCode {
            get { return _countryCode; }
            set {
                if (_countryCode != value) {
                    _countryCode = value;
                    this.OnPropertyChanged("CountryCode");
                }
            }
        }

        public String Name {
            get { return _name; }
            set {
                if (_name != value) {
                    _name = value;
                    this.OnPropertyChanged("Name");
                }
            }
        }

        public bool IsSelected {
            get { return _isSelected; }
            set {
                if (_isSelected != value) {
                    _isSelected = value;
                    this.OnPropertyChanged("IsSelected");
                }
            }
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
