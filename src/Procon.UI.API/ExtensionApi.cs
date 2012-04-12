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
using System.Windows.Controls;
using System.Windows.Media;

namespace Procon.UI.API
{
    /// <summary>A class used to interface easily between extensions and the UI.</summary>
    public static class ExtensionApi
    {
        /// <summary>
        /// Finds the control that is a child in the visual tree of another control that
        /// has a specific name and attemps to cast it as a specific type before returning it.
        /// </summary>
        /// <typeparam name="T">The type of control to cast to.</typeparam>
        /// <param name="controlAncestor">The root node to start searching the visual tree at.</param>
        /// <param name="controlName">The name of the control we're looking for.</param>
        /// <returns>Null if error/not found.  Otherwise, returns the control casted as type T.</returns>
        public static T FindControl<T>(DependencyObject controlAncestor, String controlName) where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (controlAncestor == null || String.IsNullOrEmpty(controlName))
                return null;

            // Check to see if it is a content control...
            // Otherwise, check all of it's children to see if they're what we're looking for.
            ContentControl controlAncestorCC = controlAncestor as ContentControl;
            if (controlAncestorCC != null)
            {
                // Get it's content and see if it's the type of element we're looking for.
                FrameworkElement controlContent = (controlAncestorCC.Content as FrameworkElement);
                if (controlContent != null)
                {
                    // Check to see if the names match...
                    // Otherwise, check this control's children for a match.
                    if (controlContent.Name == controlName)
                        return controlContent as T;
                    else
                    {
                        T controlContentChild = FindControl<T>(controlContent, controlName);
                        if (controlContentChild != null)
                            return controlContentChild;
                    }
                }
            }
            else
            {
                // Get the number of children this control has and iterate through them all.
                Int32 childCount = VisualTreeHelper.GetChildrenCount(controlAncestor);
                for (Int32 i = 0; i < childCount; i++)
                {
                    // Get the current child and...
                    // Compare the child's name to see if this child is the control we're looking for.
                    FrameworkElement child = VisualTreeHelper.GetChild(controlAncestor, i) as FrameworkElement;
                    if (child != null)
                    {
                        // Check to see if the names match...
                        // Otherwise, check this control's children for a match.
                        if (child.Name == controlName)
                            return child as T;
                        else
                        {
                            T childChild = FindControl<T>(child, controlName);
                            if (childChild != null)
                                return childChild;
                        }
                    }
                }
            }

            // Return Child not found.
            return null;
        }
    }
}
