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
using System.Windows.Controls;
using System.Windows.Input;

using Procon.UI.API.ViewModels;

namespace Procon.UI.Default.Root.Connection.Tabs.Bans
{
    public partial class BansView : TabItem
    {
        public BansView() { InitializeComponent(); }

        /* Reason for placing these events:
         * I must place this event in code-behind because to force the bans to update whenever the filter text changes,
         * I need a reference to the default view of the collection view source that the data grid is bound to.  To
         * accomplish this, I must either alter my InfinityDictionary to contain dependency properties (which makes
         * things a ton more convoluted) and have the ConnectionViewModel listen for changes to the filter text... then
         * refresh the filter on the Bans collection (which would be bound to in the code behind...)
         * OR
         * Simply create a "refresh filters" command, and use the TextChanged event on the textbox to call the refresh
         * command.  So, you do the extra work if you want, Macx! */

        /// <summary>Calls the "refresh filters" command every time the criteria for filtering the bans changes.</summary>
        private void FilterChatCriteriaChanged(object sender, EventArgs e)
        {
            try { ((ICommand)InstanceViewModel.PublicCommands["Connection"]["Filter"]["Ban"].Value).Execute(ConnectionTabsBansList.ItemsSource); } 
            catch (Exception) { }
        }
    }
}
