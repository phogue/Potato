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

namespace Procon.UI.Old.Enumerations
{
    /// <summary>
    /// The types of filters we can use when filtering
    /// the chatbox on the Game Window.
    /// </summary>
    public enum ChatFilterType
    {
        /// <summary>
        /// The time the message was sent.
        /// </summary>
        Time,
        /// <summary>
        /// The type of message (say, yell, etc).
        /// </summary>
        Type,
        /// <summary>
        /// Who sent the message (Player).
        /// </summary>
        Sender,
        /// <summary>
        /// What group received the message (All, Team, Squad, Player, etc).
        /// </summary>
        ReceivingSubset,
        /// <summary>
        /// Who received the message (Team1, Team2, Player1, etc).
        /// </summary>
        ReceivingEntity,
        /// <summary>
        /// The message that was sent.
        /// </summary>
        Message
    }
}
