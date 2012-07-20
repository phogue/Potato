using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Threading;

namespace Procon.UI.API.Utils
{
    // Class.
    [Serializable]
    public class NotifiableCollection<T> : IList<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        // Internal Constants.
        protected const String nCountName   = "Count";
        protected const String nIndexerName = "Item[]";

        // Internal Variables.
        private List<T> mItems     = new List<T>();
        private Boolean mNotifyOn  = true;
        private Boolean mNotifying = false;

        [field:NonSerialized]
        private Dispatcher mDispatcher = null;

        // Public Properties.
        public List<T> Items
        {
            get { return mItems; }
        }
        public Int32   Count
        {
            get { return mItems.Count; }
        }


        // Constructors.
        public NotifiableCollection()
        {
            mDispatcher = Dispatcher.CurrentDispatcher;
        }
        public NotifiableCollection(IList<T> items) : this()
        {
            if (items == null)
                throw new ArgumentNullException("items");
            AddRange(items);
        }
        public NotifiableCollection(IEnumerable<T> items) : this()
        {
            if (items == null)
                throw new ArgumentNullException("items");
            foreach (T item in items)
                Add(item);
        }

        // Various methods.
        public Boolean Contains(T item)
        {
            return mItems.Contains(item);
        }
        public Int32 IndexOf(T item)
        {
            return mItems.IndexOf(item);
        }
        public void CopyTo(T[] array, Int32 arrayIndex)
        {
            mItems.CopyTo(array, arrayIndex);
        }

        // Indexer.
        public T this[Int32 index]
        {
            get { return mItems[index];  }
            set { mItems[index] = value; }
        }
        
        // Single methods.
        public void Clear()
        {
            // Check Stuff.
            CheckNotifying();
            ClearItems();
        }
        public void Add(T item)
        {
            // Check Stuff.
            CheckNotifying();
            AddItem(item);
        }
        public void Insert(Int32 index, T item)
        {
            // Check Stuff.
            CheckNotifying();
            if (index < 0 || index > Count)
                throw new ArgumentOutOfRangeException("index");
            InsertItem(index, item);
        }
        public void Set(Int32 index, T item)
        {
            // Check Stuff.
            CheckNotifying();
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException("index");
            SetItem(index, item);
        }
        public void Move(Int32 oldIndex, Int32 newIndex)
        {
            // Check Stuff.
            CheckNotifying();
            if (oldIndex < 0 || oldIndex >= Count)
                throw new ArgumentOutOfRangeException("oldIndex");
            if (newIndex < 0 || newIndex >= Count)
                throw new ArgumentOutOfRangeException("newIndex");
            MoveItem(oldIndex, newIndex);
        }
        public bool Remove(T item)
        {
            // Check Stuff.
            Int32   index;
            Boolean contained;
            CheckNotifying();
            if ((contained = (index = mItems.IndexOf(item)) >= 0))
                RemoveItem(index);
            return contained;
        }
        public void RemoveAt(Int32 index)
        {
            // Check Stuff.
            CheckNotifying();
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException("index");
            RemoveItem(index);
        }

        // Bulk methods.
        public void AddRange(IList<T> items)
        {
            // Check Stuff.
            CheckNotifying();
            if (items == null)
                throw new ArgumentNullException("items");
            AddItems(items);
        }
        public void InsertRange(Int32 index, IList<T> items)
        {
            // Check Stuff.
            CheckNotifying();
            if (index < 0 || index > Count)
                throw new ArgumentOutOfRangeException("index");
            if (items == null)
                throw new ArgumentNullException("items");
            InsertItems(index, items);
        }
        public void SetRange(Int32 index, IList<T> items)
        {
            // Check Stuff.
            CheckNotifying();
            if (items == null)
                throw new ArgumentNullException("items");
            if (index < 0 || index >= Count || index + items.Count > Count)
                throw new ArgumentOutOfRangeException("index");
            SetItems(index, items);
        }
        public void MoveRange(Int32 oldIndex, Int32 newIndex, Int32 count)
        {
            // Check Stuff.
            CheckNotifying();
            if (oldIndex < 0 || oldIndex >= Count)
                throw new ArgumentOutOfRangeException("oldIndex");
            if (count < 0 || oldIndex + count > Count)
                throw new ArgumentOutOfRangeException("count");
            if (newIndex < 0 || newIndex + count > Count)
                throw new ArgumentOutOfRangeException("newIndex");
            MoveItems(oldIndex, newIndex, count);
        }
        public void RemoveRange(Int32 index, Int32 count)
        {
            // Check Stuff.
            CheckNotifying();
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException("index");
            if (count < 0 || index + count > Count)
                throw new ArgumentOutOfRangeException("count");
            RemoveItems(index, count);
        }

        // Back-end methods.
        protected virtual void ClearItems()
        {
            // Modify Stuff.
            foreach (T item in mItems.Where(x => x is INotifyPropertyChanged))
                ((INotifyPropertyChanged)item).PropertyChanged -= InternalItemChanged;
            mItems.Clear();

            // Notify Stuff.
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            OnPropertyChanged(new PropertyChangedEventArgs(nCountName));
            OnPropertyChanged(new PropertyChangedEventArgs(nIndexerName));
        }
        protected virtual void AddItem(T item)
        {
            InsertItem(Count, item);
        }
        protected virtual void AddItems(IList<T> items)
        {
            InsertItems(Count, items);
        }
        protected virtual void InsertItem(Int32 index, T item)
        {
            // Modify Stuff.
            mItems.Insert(index, item);
            if (item is INotifyPropertyChanged)
                ((INotifyPropertyChanged)item).PropertyChanged += InternalItemChanged;

            // Notify Stuff.
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
            OnPropertyChanged(new PropertyChangedEventArgs(nCountName));
            OnPropertyChanged(new PropertyChangedEventArgs(nIndexerName));
        }
        protected virtual void InsertItems(Int32 index, IList<T> items)
        {
            // Modify Stuff.
            mItems.InsertRange(index, items);
            foreach (T item in items)
                if (item is INotifyPropertyChanged)
                    ((INotifyPropertyChanged)item).PropertyChanged += InternalItemChanged;

            // Notify Stuff.
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (IList)new List<T>(items), index));
            OnPropertyChanged(new PropertyChangedEventArgs(nCountName));
            OnPropertyChanged(new PropertyChangedEventArgs(nIndexerName));
        }
        protected virtual void SetItem(Int32 index, T item)
        {
            // Modify Stuff.
            T oldItem = mItems[index];
            if (oldItem is INotifyPropertyChanged)
                ((INotifyPropertyChanged)oldItem).PropertyChanged -= InternalItemChanged;
            mItems[index] = item;
            T newItem = mItems[index];
            if (newItem is INotifyPropertyChanged)
                ((INotifyPropertyChanged)newItem).PropertyChanged += InternalItemChanged;

            // Notify Stuff.
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItem, oldItem, index));
            OnPropertyChanged(new PropertyChangedEventArgs(nIndexerName));
        }
        protected virtual void SetItems(Int32 index, IList<T> items)
        {
            // Modify Stuff.
            IList<T> oldItems = new List<T>(mItems.Where((x, y) => y >= index && y < index + items.Count));
            IList<T> newItems = items;
            for (int i = 0; i < items.Count; i++) {
                if (mItems[index + i] is INotifyPropertyChanged)
                    ((INotifyPropertyChanged)mItems[index + i]).PropertyChanged -= InternalItemChanged;
                mItems[index + i] = items[i];
                if (mItems[index + i] is INotifyPropertyChanged)
                    ((INotifyPropertyChanged)mItems[index + i]).PropertyChanged += InternalItemChanged;
            }

            // Notify Stuff.
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, (IList)newItems, (IList)oldItems, index));
            OnPropertyChanged(new PropertyChangedEventArgs(nIndexerName));
        }
        protected virtual void MoveItem(Int32 oldIndex, Int32 newIndex)
        {
            // Modify Stuff.
            DisableNotifying();
            T item = mItems[oldIndex];
            RemoveItem(oldIndex);
            InsertItem(newIndex, item);
            EnableNotifying();

            // Notify Stuff.
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, item, newIndex, oldIndex));
            OnPropertyChanged(new PropertyChangedEventArgs(nIndexerName));
        }
        protected virtual void MoveItems(Int32 oldIndex, Int32 newIndex, Int32 count)
        {
            // Modify Stuff.
            DisableNotifying();
            IList<T> items = new List<T>(mItems.Where((x, y) => y >= oldIndex && y < oldIndex + count));
            RemoveItems(oldIndex, count);
            InsertItems(newIndex, items);
            EnableNotifying();

            // Notify Stuff.
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, (IList)items, newIndex, oldIndex));
            OnPropertyChanged(new PropertyChangedEventArgs(nIndexerName));
        }
        protected virtual void RemoveItem(Int32 index)
        {
            // Modify Stuff.
            T item = mItems[index];
            if (item is INotifyPropertyChanged)
                ((INotifyPropertyChanged)item).PropertyChanged -= InternalItemChanged;
            mItems.RemoveAt(index);

            // Notify Stuff.
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
            OnPropertyChanged(new PropertyChangedEventArgs(nCountName));
            OnPropertyChanged(new PropertyChangedEventArgs(nIndexerName));
        }
        protected virtual void RemoveItems(Int32 index, Int32 count)
        {
            // Modify Stuff.
            IList<T> items = new List<T>(mItems.Where((x, y) => y >= index && y < index + count));
            foreach (T item in items)
                if (item is INotifyPropertyChanged)
                    ((INotifyPropertyChanged)item).PropertyChanged -= InternalItemChanged;
            mItems.RemoveRange(index, count);

            // Notify Stuff.
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, (IList)items, index));
            OnPropertyChanged(new PropertyChangedEventArgs(nCountName));
            OnPropertyChanged(new PropertyChangedEventArgs(nIndexerName));
        }

        // Helper methods.
        protected void CheckNotifying()
        {
            if (mNotifying)
                throw new InvalidOperationException("Cannot modify the collection during a Notification Changed event.");
        }
        protected void DisableNotifying()
        {
            mNotifyOn = false;
        }
        protected void EnableNotifying()
        {
            mNotifyOn = true;
        }

        #region ICollection

        Boolean ICollection<T>.IsReadOnly
        {
            get { return false; }
        }

        #endregion
        #region IEnumerable

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return mItems.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return mItems.GetEnumerator();
        }

        #endregion


        // Events.
        [field: NonSerialized]
        public event ItemChangedEventHandler ItemChanged;
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        [field: NonSerialized]
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        protected virtual void OnItemChanged(ItemChangedEventArgs args)
        {
            if (ItemChanged != null) {
                if (Dispatcher.CurrentDispatcher != mDispatcher) {
                    mDispatcher.Invoke(ItemChanged, this, args);
                    return;
                }
                ItemChanged(this, args);
            }
        }
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            if (mNotifyOn) {
                if (PropertyChanged != null) {
                    if (Dispatcher.CurrentDispatcher != mDispatcher) {
                        mDispatcher.Invoke(PropertyChanged, this, args);
                        return;
                    }
                    PropertyChanged(this, args);
                }
            }
        }
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (mNotifyOn) {
                mNotifying = true;
                if (CollectionChanged != null) {
                    if (Dispatcher.CurrentDispatcher != mDispatcher) {
                        mDispatcher.Invoke(CollectionChanged, this, args);
                        mNotifying = false;
                        return;
                    }
                    CollectionChanged(this, args);
                }
                mNotifying = false;
            }
        }

        private void InternalItemChanged(Object sender, PropertyChangedEventArgs args)
        {
            OnItemChanged(new ItemChangedEventArgs(sender, mItems.IndexOf((T)sender), args.PropertyName));
        }
    }

    // Event Args.
    public class ItemChangedEventArgs : PropertyChangedEventArgs
    {
        // Properties.
        public Object Item  { get; protected set; }
        public Int32  Index { get; protected set; }

        // Constructor.
        public ItemChangedEventArgs(Object item, Int32 index, String propertyName) : base(propertyName)
        {
            Item  = item;
            Index = index;
        }
    }
    // Event Handler.
    public delegate void ItemChangedEventHandler(Object sender, ItemChangedEventArgs e);
}

