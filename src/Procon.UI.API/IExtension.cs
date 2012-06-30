using System;
using System.Windows;

namespace Procon.UI.API
{
    public interface IExtension
    {
        // Should return the name of the author of the extension.
        String Author { get; }
        // Should return the URL to the author's website.
        String Link { get; }
        // Should return the text for the link to the author's website.
        String LinkText { get; }
        // Should return the name of the extension.
        String Name { get; }
        // Should return the version of the extension.
        String Version { get; }
        // Should return a brief description of the extension.
        String Description { get; }

        // The main entry point into the extension.
        Boolean Entry(Window root);
    }
}
