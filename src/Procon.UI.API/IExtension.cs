using System;
using System.Windows;

namespace Procon.UI.API
{
    public interface IExtension
    {
        /// <summary>
        /// Name of the author of this extension.
        /// </summary>
        String Author { get; }

        /// <summary>
        /// Address of the author's website.
        /// </summary>
        Uri Link { get; }

        /// <summary>
        /// Visible text for the author's website link.
        /// </summary>
        String LinkText { get; }


        /// <summary>
        /// Name of the extension.
        /// </summary>
        String Name { get; }

        /// <summary>
        /// Description of the extension.
        /// </summary>
        String Description { get; }

        /// <summary>
        /// Version of the extension.
        /// </summary>
        Version Version { get; }


        /// <summary>
        /// The main entry point for the extension.
        /// </summary>
        Boolean Entry(Window root);
    }
}
