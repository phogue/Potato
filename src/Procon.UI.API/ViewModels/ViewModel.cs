using System;
using System.ComponentModel;
using System.Windows.Threading;

namespace Procon.UI.API.ViewModels
{
    // Allows any class to be converted into a view model.
    public abstract class ViewModel<TModel> : INotifyPropertyChanged where TModel : class
    {
        // Variables.
        private   Dispatcher mDispatcher;
        protected TModel     nModel { get; private set; }

        
        // Constructor.
        public ViewModel(TModel model)
        {
            nModel      = model;
            mDispatcher = Dispatcher.CurrentDispatcher;
        }


        // Checks to see if the model is the same as the one specified.
        internal Boolean ModelEquals(TModel model)
        {
            return nModel == model;
        }
        // Changes to the original dispatcher if the current one is different.
        internal Boolean ChangeDispatcher(Action method)
        {
            if (Dispatcher.CurrentDispatcher != mDispatcher) {
                mDispatcher.Invoke(method);
                return true;
            }
            return false;
        }

        
        #region INotifyPropertyChanged

        // Allows for external applications/classes to know when properties have been updated.
        public    event PropertyChangedEventHandler PropertyChanged;
        protected void  OnPropertyChanged(String property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        #endregion
    }
}
