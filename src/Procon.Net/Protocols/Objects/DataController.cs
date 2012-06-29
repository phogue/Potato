using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Procon.Net.Protocols.Objects
{
    /// <summary>
    /// An implementation used to create a back-end for storing data in a
    /// particular class. Allows data abstraction to be used when accessing
    /// data by using virtual overrides to access data in child classes.
    /// </summary>
    /// <remarks>
    /// By using a data controller as the back end for data accessing and
    /// mutating, an abstraction layer is created that allows sub classes
    /// (e.g. Player > FrostbitePlayer) to access data by overriding public
    /// virtual properties declared in Player.
    /// 
    /// Furthermore, data not exposed by the common properties can still be
    /// accessed.  This allows data to be added to a single class without
    /// being required to update every child class.
    /// </remarks>
    [Serializable]
    public class DataController : INotifyPropertyChanged
    {
        // Properties.
        public  List<DataVariable> Variables {
            get { return mVariables; }
            set {
                if (mVariables != value) {
                    mVariables = value;
                    OnPropertyChanged("Variables");
        } } }
        private List<DataVariable> mVariables = new List<DataVariable>();


        // Front-end "Single" accessor/mutator for the data.
        public DataVariable this[String key]
        {
            get
            {
                if (!DataContains(key)) {
                    DataVariable vKey = new DataVariable(key);
                    _DataAdd(vKey);
                    return vKey;
                }
                return mVariables.First(x => x.Name == key);
            }
            set { _DataSet(this[key], value != null ? value : new DataVariable(key)); }
        }

        // Mutators for the data.
        public void DataSet(DataVariable value)
        {
            if (value != null && value.Name != null)
                _DataSet(this[value.Name], value);
        }
        public void DataSet(String key, Object value)
        {
            _DataSet(this[key], value);
        }
        public void DataRemove(String key)
        {
            if (DataContains(key))
                _DataRemove(this[key]);
        }
        public void DataClear()
        {
            mVariables.Clear();
        }

        // Back-end implementation for the mutators.
        private void _DataAdd(DataVariable value)
        {
            mVariables.Add(value);
            OnDataAdded(this, value);
            value.PropertyChanged += Data_PropertyChanged;
        }
        private void _DataSet(DataVariable oValue, DataVariable nValue)
        {
            oValue.Value        = nValue.Value;
            oValue.IsReadOnly   = nValue.IsReadOnly;
            oValue.FriendlyName = nValue.FriendlyName;
            oValue.Description  = nValue.Description;
        }
        private void _DataSet(DataVariable oValue, Object nValue)
        {
            oValue.Value = nValue;
        }
        private void _DataRemove(DataVariable value)
        {
            mVariables.Remove(value);
            OnDataRemoved(this, value);
            value.PropertyChanged -= Data_PropertyChanged;
        }


        // Data Validation.
        public Int32   DataCount { get { return mVariables.Count; } }
        public Boolean DataContains(String key) { return mVariables.FirstOrDefault(x => x.Name == key) != null; }

        // Data Validation Accessor.
        public T TryGetVariable<T>(String key, T fallback = default(T))
        {
            if (DataContains(key))
                return this[key].TryGetValue<T>(fallback);
            return fallback;
        }

        // Data Notified Property Changed events.
        private void Data_PropertyChanged(Object sender, PropertyChangedEventArgs e)
        {
            OnDataChanged(this, (DataVariable)sender);
        }


        // Events.
        public delegate void DataAlteredHandler(DataController parent, DataVariable item);
        [field: NonSerialized]
        public event DataAlteredHandler DataAdded;
        [field: NonSerialized]
        public event DataAlteredHandler DataRemoved;
        [field: NonSerialized]
        public event DataAlteredHandler DataChanged;
        protected void OnDataAdded(DataController parent, DataVariable item)
        {
            if (DataAdded != null)
                DataAdded(parent, item);
        }
        protected void OnDataRemoved(DataController parent, DataVariable item)
        {
            if (DataRemoved != null)
                DataRemoved(parent, item);
        }
        protected void OnDataChanged(DataController parent, DataVariable item)
        {
            if (DataChanged != null)
                DataChanged(parent, item);
        }

        #region INotifyPropertyChanged

        // Allows for external applications/classes to know when properties have been updated.
        [field: NonSerialized]
        public    event PropertyChangedEventHandler PropertyChanged;
        protected void  OnPropertyChanged(String property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        #endregion
    }
}
