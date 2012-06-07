using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Procon.UI.API.Classes
{
    // An array of arrays that works similar to a "Map" in C++ or "Array" in PhP.
    public class InfinityDictionary<TKey, TValue> : INotifyPropertyChanged
    {
        // Private Variables.
        private TValue mValue;
        private Dictionary<TKey, InfinityDictionary<TKey, TValue>> mArray;

        // Public Accessors/Mutators.
        public TValue Value
        {
            get { return mValue; }
            set {
                mValue = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Value"));
            }
        }
        public ICollection<TKey> Keys
        {
            get { return mArray.Keys; }
        }
        public Int32 Count
        {
            get { return mArray.Count; }
        }


        // Constructors.
        public InfinityDictionary(TValue value = default(TValue))
        {
            mValue = value;
            mArray = new Dictionary<TKey, InfinityDictionary<TKey, TValue>>();
        }

        // Get: Adds the entry if it doesn't exist, then returns the entry.
        // Set: Adds the entry if it doesn't exist, then replaces the entry.
        public InfinityDictionary<TKey, TValue> this[TKey key]
        {
            get {
                if (!mArray.ContainsKey(key))
                    mArray.Add(key, new InfinityDictionary<TKey, TValue>());
                return mArray[key];
            }
            set {
                if (mArray.ContainsKey(key))
                    mArray.Remove(key);
                mArray.Add(key, value);
            }
        }

        // Adds/Sets the entry, but will not replace/add an old/new entry.
        public void Add(TKey key, TValue value)
        {
            if (!mArray.ContainsKey(key))
                mArray.Add(key, new InfinityDictionary<TKey, TValue>(value));
        }
        public void Set(TKey key, TValue value)
        {
            if (mArray.ContainsKey(key))
                mArray[key].Value = value;
        }

        // Removes the key/all keys from the dictionary.
        public Boolean Remove(TKey key)
        {
            return mArray.Remove(key);
        }
        public void RemoveAll()
        {
            mArray.Clear();
        }

        // Checks to see if the key exists.
        public Boolean ContainsKey(TKey key)
        {
            return mArray.ContainsKey(key);
        }


        // Events.
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
