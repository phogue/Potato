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

namespace Procon.Net.Protocols.Objects
{
    /// <summary>
    /// Contains information about and represents data received from a game server.
    /// </summary>
    [Serializable]
    public class DataVariable : INotifyPropertyChanged
    {
        // Back-end implementation.
        private String  mName;
        private Object  mValue;
        private Boolean mIsReadOnly;
        private String  mFriendlyName;
        private String  mDescription;

        // Front-end accessors/mutators.
        public String  Name
        {
            get { return mName; }
            set {
                if (mName != value) {
                    mName = value;
                    OnPropertyChanged("Name");
                }
            }
        }
        public Object  Value
        {
            get { return mValue; }
            set {
                if (mValue != value) {
                    mValue = value;
                    OnPropertyChanged("Value");
                }
            }
        }
        public Boolean IsReadOnly
        {
            get { return mIsReadOnly; }
            set {
                if (mIsReadOnly != value) {
                    mIsReadOnly = value;
                    OnPropertyChanged("IsReadOnly");
                }
            }
        }
        public String  FriendlyName
        {
            get { return mFriendlyName != null ? mFriendlyName : mName; }
            set
            {
                if (mFriendlyName != value)
                {
                    mFriendlyName = value;
                    OnPropertyChanged("FriendlyName");
                }
            }
        }
        public String  Description
        {
            get { return mDescription; }
            set
            {
                if (mDescription != value)
                {
                    mDescription = value;
                    OnPropertyChanged("Description");
                }
            }
        }

        // Constructor.
        public DataVariable(String name = null, Object value = null, Boolean isReadOnly = false, String friendlyName = null, String description = null)
        {
            mName         = name;
            mValue        = value;
            mIsReadOnly   = isReadOnly;
            mFriendlyName = friendlyName;
            mDescription  = description;
        }
        
        // Error handling type accessor.
        public T TryGetValue<T>(T fallback = default(T))
        {
            if (Value != null) {
                try {
                    return (T)Value;
                }
                catch (Exception) { }
            }

            return fallback;
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
    }
}
