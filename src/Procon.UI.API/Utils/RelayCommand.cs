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
using System.Windows.Input;

namespace Procon.UI.API.Utils
{
    /// <summary>
    /// A command that is not bound by the visual tree to determine whether
    /// it is able to be executed or not.  The methods to determine whether
    /// the command is able to be executed and to execute when the command
    /// is executed are specified when the command is constructed.
    /// </summary>
    /// <typeparam name="T">The type of the parameters the command should take.<The /typeparam>
    public class RelayCommand<T> : ICommand where T : class
    {
        #region Fields

        private readonly Action<T>    mExecute;
        private readonly Predicate<T> mCanExecute;

        #endregion // Fields
        #region Constructors

        /// <summary>
        /// Creates a command whose parameter is of type T, and execute/can
        /// execute actions are determined by the methods that are passed
        /// into the constructor.
        /// </summary>
        /// <param name="executeTarget">The method to use when the command is executed.</param>
        /// <param name="canExecuteTarget">The method to use to see if the command can be executed.</param>
        public RelayCommand(Action<T> executeTarget, Predicate<T> canExecuteTarget = null)
        {
            if (executeTarget == null)
                throw new ArgumentNullException("executeTarget");

            mExecute    = executeTarget;
            mCanExecute = canExecuteTarget;
        }

        #endregion // Constructors
        #region ICommand Members

        /// <summary>
        /// Calls the "can execute" method specified when the command was constructed
        /// to see if the command is able to be executed.
        /// </summary>
        /// <param name="parameter">A parameter used to determine if the command can execute.</param>
        /// <returns>The result of the "can execute target" method specified at construction.</returns>
        public bool CanExecute(Object parameter)
        {
            if (mCanExecute != null)
                return mCanExecute((T)parameter);
            return true;
        }

        /// <summary>
        /// Fires when the commands ability to execute the command has changed.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add    { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Calls the "execute" method specified when the command was constructed.
        /// </summary>
        /// <param name="parameter">A parameter used by the command during execution.</param>
        public void Execute(Object parameter)
        {
            if (CanExecute(parameter))
                mExecute((T)parameter);
        }

        #endregion // ICommand Members
    }
}