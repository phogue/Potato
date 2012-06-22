using System;
using System.Windows.Markup;

namespace Procon.UI.API.Converters
{
    public abstract class BaseConverter : MarkupExtension
    {
        // Markup Extension.
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
