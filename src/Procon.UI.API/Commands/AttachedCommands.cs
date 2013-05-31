using System.Windows;

namespace Procon.UI.API.Commands
{
    public sealed class AttachedCommands
    {
        // Necessary to get the events bound to the element.
        private static readonly DependencyPropertyKey RoutinesPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly("RoutinesInternal",
                typeof(AttachedCommandCollection),
                typeof(AttachedCommands),
                new FrameworkPropertyMetadata((AttachedCommandCollection)null));
        public static readonly DependencyProperty RoutinesProperty = RoutinesPropertyKey.DependencyProperty;

        public static void SetRoutines(DependencyObject element, AttachedCommandCollection collection)
        {
            element.SetValue(RoutinesPropertyKey, collection);
        }
        public static AttachedCommandCollection GetRoutines(DependencyObject element)
        {
            AttachedCommandCollection collection = (AttachedCommandCollection)element.GetValue(RoutinesProperty);
            if (collection == null) {
                collection = new AttachedCommandCollection() { Element = element };
                SetRoutines(element, collection);
            }
            return collection;
        }
    }
}
