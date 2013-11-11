using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Procon.Setup {
    public class RelayCommand : ICommand {
        private readonly Action<Object> _action;

        public RelayCommand(Action<Object> action) {
            _action = action;
        }

        public bool CanExecute(object parameter) {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter) {
            if (parameter != null) {
                _action(parameter);
            }
            else {
                _action(null);
            }
        }
    }
}
