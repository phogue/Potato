using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Threading;

namespace Procon.UI.API.ViewModels
{
    // Allows any class to be converted into a view model.
    public abstract class ViewModel<TModel> : ViewModelBase, INotifyPropertyChanged where TModel : class
    {
        // Private Variables.
        private Dispatcher     mDispatcher;
        private PropertyInfo[] mProperties;

        // The Model.
        protected TModel Model { get; private set; }

        
        // Constructor.
        public ViewModel(TModel model)
        {
            Model       = model;
            mDispatcher = Dispatcher.CurrentDispatcher;
            mProperties = GetType().GetProperties();
        }

        // Checks to see if the model is the same as the one specified.
        public Boolean ModelEquals(TModel model)
        {
            return Model == model;
        }

        // Changes to the original dispatcher if the current one is different.
        public Boolean ChangeDispatcher(Action method)
        {
            if (Dispatcher.CurrentDispatcher != mDispatcher) {
                mDispatcher.Invoke(method);
                return true;
            }
            return false;
        }

        // Forces the OnPropertyChanged event to be called for all properties in the view model.
        public void RebindAll()
        {
            foreach (PropertyInfo prop in mProperties)
                OnPropertyChanged(this, prop.Name);
        }


        // Events.
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(Object sender, String property)
        {
            if (PropertyChanged != null)
                PropertyChanged(sender, new PropertyChangedEventArgs(property));
        }
    }
}
