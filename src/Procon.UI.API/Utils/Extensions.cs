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
using System.IO;

namespace Procon.UI.API.Utils
{
    /// <summary>Various class extensions built for the UI namespace.</summary>
    public static class Extensions
    {
        /// <summary>
        /// Determines the relative path from the given string to the string the method
        /// was called from.  Cleans up the string so that it is human readable.
        /// </summary>
        /// <param name="to">The string we want to make relative.</param>
        /// <param name="from">The string we are going to make it relative to.</param>
        public static String GetRelativePath(this String to, String from)
        {
            // Simply return the original string if bad parameter.
            if (String.IsNullOrEmpty(from)) return to;

            // Gets the relative path.
            String relative = new Uri(from).MakeRelativeUri(new Uri(to)).ToString();

            // Cleans up the string and returns it.
            return Uri.UnescapeDataString(relative).Replace('/', Path.DirectorySeparatorChar);
        }
    }
}
