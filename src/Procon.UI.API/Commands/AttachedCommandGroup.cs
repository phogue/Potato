using System.Collections.Generic;
using System.Windows;

namespace Procon.UI.API.Commands
{
    public sealed class AttachedCommandGroup : FreezableCollection<AttachedCommandItem>
    {
        private readonly HashSet<AttachedCommandItem> mItems = new HashSet<AttachedCommandItem>();

        // Attaches the event handler for each item to it's appropriate command.
        public void AttachItems(FrameworkElement element)
        {
            foreach (AttachedCommandItem item in this)
                if (!mItems.Contains(item)) {
                    item.AttachItem(element);
                    mItems.Add(item);
                }
        }
    }
}
