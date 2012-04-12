// Copyright 2011 Cameron 'Imisnew2' Gunnin
// 
// http://www.phogue.net
//  
// This file is part of Procon 2.
// 
// Procon 2 is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Procon 2 is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Procon 2.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Threading;

using Procon.UI.API.Utils;

namespace Procon.UI.API.ViewModels
{
    /// <summary>
    /// Wraps a model so that a UI can access and display various elements of the model
    /// without needing to bind/rely on updates directly to/from the model itself.
    /// </summary>
    /// <typeparam name="TModel">The type of the model stored in this view model.</typeparam>
    public abstract class ViewModel<TModel> : INotifyPropertyChanged where TModel : class
    {
        public static InfinityDictionary<String, Object>   PublicProperties { get; set; }
        public static InfinityDictionary<String, ICommand> PublicCommands   { get; set; }

        protected TModel     Model      { get; private set; }
        protected Dispatcher MainQueue  { get; private set; }
        private PropertyInfo[] mProperties;

        /// <summary>
        /// Creates an instance of a ViewModel and initializes all the properties using the
        /// given model's instance.
        /// </summary>
        /// <param name="model">The model that this view model wraps.</param>
        public ViewModel(TModel model)
        {
            Model       = model;
            MainQueue   = Dispatcher.CurrentDispatcher;
            mProperties = GetType().GetProperties();
            if (PublicProperties == null)
                PublicProperties = new InfinityDictionary<String, Object>();
            if (PublicCommands == null)
                PublicCommands = new InfinityDictionary<String, ICommand>();
        }

        /// <summary>
        /// Checks to see if the specified model is the same reference as the internal model.
        /// </summary>
        /// <param name="model">The model to check,</param>
        public Boolean ModelEquals(TModel model)
        {
            return Model == model;
        }

        /// <summary>
        /// Fires the PropertyChanged handler for each of the properties in this instance.
        /// Used to easily fire the OnPropertyChanged events for each property in the this
        /// instance, if necessary.
        /// </summary>
        public void RebindAll()
        {
            foreach (PropertyInfo prop in mProperties)
                OnPropertyChanged(this, prop.Name);
        }

        #region INotifyPropertyChanged

        /// <summary>
        /// Is fired whenever a property in this instance changes.  If a property of this
        /// instance is bound to in the UI, this will tell the UI to update the value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// This should be used to fire the PropertyChanged handler, so that the UI
        /// (and other instances) can be notified when a property changes.
        /// </summary>
        protected void OnPropertyChanged(object sender, String property)
        {
            if (PropertyChanged != null)
                PropertyChanged(sender, new PropertyChangedEventArgs(property));
        }

        #endregion INotifyPropertyChanged
    }
}
