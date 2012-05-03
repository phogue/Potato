using System;
using System.Windows.Input;

namespace Procon.UI.API.Commands
{
    public class RelayCommand<T> : ICommand where T : class
    {
        // Private variables.
        private readonly Action<T>    mExecute;
        private readonly Predicate<T> mCanExecute;


        // Constructors.
        public RelayCommand(Action<T> executeTarget, Predicate<T> canExecuteTarget = null)
        {
            if (executeTarget == null)
                throw new ArgumentNullException("executeTarget");

            mExecute    = executeTarget;
            mCanExecute = canExecuteTarget;
        }

        // Executes the command if it determines it is allowed to execute.
        public void Execute(Object parameter)
        {
            if (CanExecute(parameter))
                mExecute((T)parameter);
        }
        public bool CanExecute(Object parameter)
        {
            if (mCanExecute != null)
                return mCanExecute((T)parameter);
            return true;
        }


        // Events.
        public event EventHandler CanExecuteChanged
        {
            add    { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}