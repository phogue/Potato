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

namespace Procon.UI.API
{
    /// <summary>
    /// This attribute allows the loader knowledge about an extension so that the loader can
    /// try to resolve conflicts that might arise between extensions.  This also allows the
    /// manager to inform the user of possible conflicts that may cause an extension to fail.
    /// For instance, if another extension replaces a control that this extension alters,
    /// then a conflict may arise if the former extension does not re-define the control.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ExtensionAttribute : Attribute
    {
        /// <summary>
        /// A List of extensions that must be executed before this extension is executed.
        /// This is used because simply specifying the control name in "Alters" does not
        /// tell the loader which extension the control came from.
        /// </summary>
        public String[] DependsOn { get; set; }
        /// <summary>
        /// A list of controls that are altered by this extension.
        /// This is used to determine if conflicts may arise between extensions.
        /// </summary>
        public String[] Alters { get; set; }
        /// <summary>
        /// A list of controls that are replaced by this extension.
        /// This is used to determine if conflicts may arise between extensions.
        /// </summary>
        public String[] Replaces { get; set; }
    }
}
