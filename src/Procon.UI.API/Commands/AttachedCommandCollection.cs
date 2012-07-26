using System;
using System.Collections.Specialized;
using System.Windows;

namespace Procon.UI.API.Commands
{
    public sealed class AttachedCommandCollection : FreezableCollection<AttachedCommandItem>
    {
        // The element that controls the collection.
        public DependencyObject Element { get; internal set; }


        // Constructor.
        public AttachedCommandCollection()
        {
            ((INotifyCollectionChanged)this).CollectionChanged += AttachedCommandCollectionChanged;
        }

        // Monitors the rountine groups for changes.
        private void AttachedCommandCollectionChanged(Object sender, NotifyCollectionChangedEventArgs e)
        {
            AttachedCommandCollection group = (AttachedCommandCollection)sender;
            switch (e.Action) {
                // Link new items to the element.
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems != null)
                        foreach (AttachedCommandItem item in e.NewItems) {
                            item.Routine.Element = group.Element;
                            item.Routine.Bind();
                        }
                    break;

                // Unbind old items.
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null)
                        foreach (AttachedCommandItem item in e.OldItems)
                            item.Event = null;
                    break;

                // Combination of linking new items and unbinding old items.
                case NotifyCollectionChangedAction.Replace:
                    if (e.NewItems != null)
                        foreach (AttachedCommandItem item in e.NewItems) {
                            item.Routine.Element = group.Element;
                            item.Routine.Bind();
                        }

                    if (e.OldItems != null)
                        foreach (AttachedCommandItem item in e.OldItems)
                            item.Event = null;
                    break;

                // Unbind all the old items.
                case NotifyCollectionChangedAction.Reset:
                    if (e.OldItems != null)
                        foreach (AttachedCommandItem item in e.OldItems)
                            item.Event = null;
                    break;
            }
        }
    }
}
