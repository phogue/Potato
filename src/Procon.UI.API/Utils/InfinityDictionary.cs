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
using System.Collections.Generic;
using System.ComponentModel;

namespace Procon.UI.API.Utils
{
    /// <summary>
    /// Contains an array of arrays, where each array is dynamically created
    /// if the key does not exist when the user specifies a specific key.
    /// To return the value for a specific key, you must use the .Value
    /// property.
    /// </summary>
    /// <typeparam name="TKey">The type to use for identifiers (Keys).</typeparam>
    /// <typeparam name="TValue">The type to use for values.</typeparam>
    public class InfinityDictionary<TKey, TValue> : INotifyPropertyChanged
    {
        #region Private Variables

        /// <summary>
        /// Represents the object stored at this entry.
        /// </summary>
        private TValue mValue;
        /// <summary>
        /// Represents all of the sub entries that can exist at this entry.
        /// </summary>
        private Dictionary<TKey, InfinityDictionary<TKey, TValue>> mArray;

        #endregion Private Variables
        #region Constructors

        /// <summary>
        /// Creates a binding dictionary used to dynamically allocate entries
        /// based on the keys used.  Similar to how a map works in C++.
        /// </summary>
        public InfinityDictionary(TValue value = default(TValue))
        {
            mValue = value;
            mArray = new Dictionary<TKey, InfinityDictionary<TKey, TValue>>();
        }

        #endregion Constructors

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Get: Adds a new entry if the key does not exist then returns that
        /// entry, otherwise returns the exisiting entry.
        /// Set: Adds an entry if the key does not exist, replaces the entry
        /// if the key already exists.
        /// </summary>
        /// <param name="key">The key to search for.</param>
        public InfinityDictionary<TKey, TValue> this[TKey key]
        {
            get
            {
                if (!mArray.ContainsKey(key))
                    mArray.Add(key, new InfinityDictionary<TKey, TValue>());
                return mArray[key];
            }
            set
            {
                if (mArray.ContainsKey(key))
                    mArray.Remove(key);
                mArray.Add(key, value);
            }
        }
        /// <summary>
        /// The number of keys in this dictionary.
        /// </summary>
        public Int32 Count
        {
            get { return mArray.Count; }
        }


        /// <summary>
        /// Gets or sets the value for this entry.
        /// </summary>
        public TValue Value
        {
            get { return mValue; }
            set
            {
                mValue = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Value"));
            }
        }
        /// <summary>
        /// Gets a list of keys that currently exist.
        /// </summary>
        public ICollection<TKey> Keys
        {
            get { return mArray.Keys; }
        }
        /// <summary>
        /// Determines whether the dictionary contains the specified key.
        /// </summary>
        /// <param name="key">The key to search for.</param>
        public Boolean ContainsKey(TKey key)
        {
            return mArray.ContainsKey(key);
        }


        /// <summary>
        /// Adds the specified entry (if it does not exist) to the dictionary.
        /// </summary>
        /// <param name="key">The key to add.</param>
        public Boolean Add(TKey key, TValue value)
        {
            if (mArray.ContainsKey(key))
                return false;
            mArray.Add(key, new InfinityDictionary<TKey, TValue>(value));
            return true;
        }
        /// <summary>
        /// Removes the specified entry (and all sub-entries!) from the dictionary.
        /// </summary>
        /// <param name="key">The key to remove.</param>
        public Boolean Remove(TKey key)
        {
            return mArray.Remove(key);
        }
        /// <summary>
        /// Removes all entries (and all sub-entries!) from the dictionary.  The value
        /// of this entry is not affected.
        /// </summary>
        public void Clear()
        {
            mArray.Clear();
        }
    }
}
