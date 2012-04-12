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
using System.Windows;

namespace Procon.UI.API
{
    /// <summary>A way to get common information about an extension.</summary>
    public interface IExtension
    {
        /// <summary>Should return the name of the author of the extension.</summary>
        String Author { get; }
        /// <summary>Should return the URL to the author's website.</summary>
        String Link { get; }
        /// <summary>Should return the text for the link to the author's website.</summary>
        String LinkText { get; }
        /// <summary>Should return the name of the extension.</summary>
        String Name { get; }
        /// <summary>Should return the version of the extension.</summary>
        String Version { get; }
        /// <summary>Should return a brief description of the extension.</summary>
        String Description { get; }

        /// <summary>The main entry point into the extension.</summary>
        /// <param name="root">A reference to the root of the UI.</param>
        /// <returns>Whether the extension was executed successfully.</returns>
        Boolean Entry(Window root);
    }
}
