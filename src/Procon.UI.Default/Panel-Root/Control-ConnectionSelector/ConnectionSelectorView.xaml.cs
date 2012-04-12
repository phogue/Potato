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
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Procon.UI.Default.Root.ConnectionSelector
{
    public partial class ConnectionSelectorView : StackPanel
    {
        public ConnectionSelectorView() { InitializeComponent(); }

        /* Reason for placing these events:
         * I must place this event in code-behind because Storyboards do not allow for binding because
         * the To and From properties must be able to be frozen.  Therefore, to create a storyboard without
         * having to use some humongous work-around, I'm just breaking MVVM for a bit.  Don't shoot me Macx. */
        private Double minWidth = 25.0;
        private Double maxWidth = 275.0;

        /// <summary>Expands the connections tab that was hovered over to the actual size of the element.</summary>
        private void ExpandConnectionTab(object sender, MouseEventArgs e)
        {
            Button tab = (sender as Button);
            if (tab == null) return;

            Storyboard story = new Storyboard();
            Storyboard.SetTarget(story, tab);
            Storyboard.SetTargetProperty(story, new PropertyPath(Button.WidthProperty));

            DoubleAnimation expand   = new DoubleAnimation();
            expand.DecelerationRatio = 1.0;
            expand.Duration          = TimeSpan.FromSeconds(0.5 * (1 - (tab.ActualWidth - minWidth) / maxWidth));
            expand.From              = tab.ActualWidth;
            expand.To                = maxWidth;

            story.Children.Add(expand);
            story.Begin();
        }
        /// <summary>Shrinks the connections tab that was hovered over to the minimum width.</summary>
        private void ShrinkConnectionTab(object sender, MouseEventArgs e)
        {
            Button tab = (sender as Button);
            if (tab == null) return;

            Storyboard story = new Storyboard();
            Storyboard.SetTarget(story, tab);
            Storyboard.SetTargetProperty(story, new PropertyPath(Button.WidthProperty));

            DoubleAnimation shrink   = new DoubleAnimation();
            shrink.DecelerationRatio = 1.0;
            shrink.Duration          = TimeSpan.FromSeconds(0.5 * ((tab.ActualWidth - minWidth) / maxWidth));
            shrink.From              = tab.ActualWidth;
            shrink.To                = minWidth;

            story.Children.Add(shrink);
            story.Begin();
        }

        /* Reason for placing these events:
         * Pretty much the same as the above.  Although, I could have done this with commands... since the functionality
         * is so similar to the other expanding tabs, I just placed it here.  I swear I'm wearing a bullet proof vest. */
        private Double minAddWidth  = 25.0;
        private Double minAddHeight = 25.0;

        /// <summary>Expands the add connection tab to view the extra elements.</summary>
        private void ExpandAddTab(object sender, RoutedEventArgs e)
        {
            Border tab  = (sender as Border);
            Grid   area = (tab.Child as Grid);
            if (tab == null || area == null) return;

            Storyboard story = new Storyboard();

            DoubleAnimation expandHeight   = new DoubleAnimation();
            expandHeight.DecelerationRatio = 1.0;
            expandHeight.Duration          = TimeSpan.FromSeconds(0.5 * (1 - (tab.ActualHeight - minAddHeight) / area.ActualHeight));
            expandHeight.From              = tab.ActualHeight;
            expandHeight.To                = area.ActualHeight;
            Storyboard.SetTarget(expandHeight, tab);
            Storyboard.SetTargetProperty(expandHeight, new PropertyPath(Border.HeightProperty));

            DoubleAnimation expandWidth   = new DoubleAnimation();
            expandWidth.DecelerationRatio = 1.0;
            expandWidth.Duration          = TimeSpan.FromSeconds(0.5 * (1 - (tab.ActualWidth - minAddWidth) / maxWidth));
            expandWidth.From              = tab.ActualWidth;
            expandWidth.To                = maxWidth;
            Storyboard.SetTarget(expandWidth, tab);
            Storyboard.SetTargetProperty(expandWidth, new PropertyPath(Border.WidthProperty));

            story.Children.Add(expandHeight);
            story.Children.Add(expandWidth);
            story.Begin();
        }
        /// <summary>Shrinks the add connection tab to hide the extra elements.</summary>
        private void ShrinkAddTab(object sender, RoutedEventArgs e)
        {
            Border tab  = (sender as Border);
            Grid   area = (tab.Child as Grid);
            if (tab == null || area == null) return;

            Storyboard story = new Storyboard();

            DoubleAnimation shrinkHeight   = new DoubleAnimation();
            shrinkHeight.DecelerationRatio = 1.0;
            shrinkHeight.Duration          = TimeSpan.FromSeconds(0.5 * ((tab.ActualHeight - minAddHeight) / area.ActualHeight));
            shrinkHeight.From              = tab.ActualHeight;
            shrinkHeight.To                = minAddHeight;
            Storyboard.SetTarget(shrinkHeight, tab);
            Storyboard.SetTargetProperty(shrinkHeight, new PropertyPath(Border.HeightProperty));

            DoubleAnimation shrinkWidth   = new DoubleAnimation();
            shrinkWidth.DecelerationRatio = 1.0;
            shrinkWidth.Duration          = TimeSpan.FromSeconds(0.5 * ((tab.ActualWidth - minAddWidth) / maxWidth));
            shrinkWidth.From              = tab.ActualWidth;
            shrinkWidth.To                = minAddWidth;
            Storyboard.SetTarget(shrinkWidth, tab);
            Storyboard.SetTargetProperty(shrinkWidth, new PropertyPath(Border.WidthProperty));

            story.Children.Add(shrinkHeight);
            story.Children.Add(shrinkWidth);
            story.Begin();
        }
    }
}
