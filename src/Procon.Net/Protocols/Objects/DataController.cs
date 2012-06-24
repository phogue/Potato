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
        // Back-end implementation.
        private List<DataVariable> mVariables = new List<DataVariable>();
        public  List<DataVariable> Variables {
            get { return mVariables; }
            set {
                if (mVariables != value) {
                    mVariables = value;
                    OnPropertyChanged("Variables");
        } } }

        // Front-end "Single" accessor/mutator for the data.
        public DataVariable this[String key]
        {
            get { return mVariables.FirstOrDefault(x => x.Name == key); }
            set
            {
                DataVariable vKey = this[key];
                if      (vKey == null && value != null) _DataAdd(value);
                else if (vKey != null && value == null) _DataRemove(vKey);
                else if (vKey != null && value != null) _DataSet(vKey, value);
            }
        }

        #region Mutators

        // Front-end mutators for the data.
        public void DataAdd(String key, DataVariable value)
        {
            DataVariable vKey = this[key];
            if (vKey == null) _DataAdd(value);
        }
        public void DataAdd(String key, Object value)
        {
            DataVariable vKey = this[key];
            if (vKey == null) _DataAdd(new DataVariable(key, value));
        }
        public void DataAddSet(String key, DataVariable value)
        {
            DataVariable vKey = this[key];
            if (vKey == null) _DataAdd(value);
            else              _DataSet(vKey, value);
        }
        public void DataAddSet(String key, Object value)
        {
            DataVariable vKey = this[key];
            if (vKey == null) _DataAdd(new DataVariable(key, value));
            else              _DataSet(vKey, value);
        }
        public void DataSet(String key, DataVariable value)
        {
            DataVariable vKey = this[key];
            if (vKey != null) _DataSet(vKey, value);
        }
        public void DataSet(String key, Object value)
        {
            DataVariable vKey = this[key];
            if (vKey != null) _DataSet(vKey, value);
        }
        public void DataRemove(String key)
        {
            DataVariable vKey = this[key];
            if (vKey != null) _DataRemove(vKey);
        }

        // Back-end mutators for the data.
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
            OnDataChanged(this, oValue);
        }
        private void _DataSet(DataVariable oValue, Object nValue)
        {
            oValue.Value = nValue;
            OnDataChanged(this, oValue);
        }
        private void _DataRemove(DataVariable value)
        {
            mVariables.Remove(value);
            OnDataRemoved(this, value);
            value.PropertyChanged -= Data_PropertyChanged;
        }

        #endregion
        #region Misc.

        // Misc. Properties.
        public Int32 DataCount { get { return mVariables.Count; } }

        // Misc. Functions.
        public Boolean DataContains(String key) { return mVariables.Count(x => x.Name == key) > 0; }
        public void    DataClear()              { mVariables.Clear(); }
        
        // Misc. Accessor.
        public T TryGetVariable<T>(String key, T fallback = default(T))
        {
            if (DataContains(key))
                try               { return this[key].TryGetValue<T>(fallback); }
                catch (Exception) { }
            return fallback;
        }

        #endregion
        #region Events

        // Data Added / Removed / Changed events.
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

        // Data Notified Property Changed events.
        private void Data_PropertyChanged(Object sender, PropertyChangedEventArgs e)
        {
            OnDataChanged(this, (DataVariable)sender);
        }

        #region INotifyPropertyChanged

        // Allows for external applications/classes to know when properties have been updated.
        [field: NonSerialized]
        public    event PropertyChangedEventHandler PropertyChanged;
        protected void  OnPropertyChanged(String name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        #endregion
    }
}
