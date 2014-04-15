#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
#region

using System.Collections.Generic;
using Potato.Core.Shared;
using Potato.Core.Shared.Database.Migrations;
using Potato.Net.Shared.Utils;

#endregion

namespace Potato.Core.Test.Database.Helpers {
    public class TestMigrationTrackerHelper {
        public TestMigrationTrackerHelper() {
            Migrations = new MigrationController() {
                // Bubble all commands to the database controller
                BubbleObjects = new List<ICoreController>() {
                    TestDatabaseController.OpenSqLiteDriver()
                },
                Migrations = new List<IMigration>() {
                    new Migration() {
                        Up = () => {
                            Tracker = 1;

                            Counter++;

                            return true;
                        },
                        Down = () => {
                            Tracker = 0;

                            Counter++;

                            return true;
                        }
                    },
                    new Migration() {
                        Up = () => {
                            Tracker = 2;

                            Counter++;

                            return true;
                        },
                        Down = () => {
                            Tracker = 1;

                            Counter++;

                            return true;
                        }
                    },
                    new Migration() {
                        Up = () => {
                            Tracker = 3;

                            Counter++;

                            return true;
                        },
                        Down = () => {
                            Tracker = 2;

                            Counter++;

                            return true;
                        }
                    },
                    new Migration() {
                        Up = () => {
                            Tracker = 4;

                            Counter++;

                            return true;
                        },
                        Down = () => {
                            Tracker = 3;

                            Counter++;

                            return true;
                        }
                    }
                },
                Settings = new MigrationSettings() {
                    // Create a random stream name
                    Name = StringExtensions.RandomString(10),
                    // Just use Core as the origin for testing
                    Origin = MigrationOrigin.Core
                }
            }.Execute() as MigrationController;
        }

        /// <summary>
        ///     Tracks the last migration called.
        /// </summary>
        public int Tracker { get; set; }

        /// <summary>
        ///     Counts how many migrations were called (up or down)
        /// </summary>
        public int Counter { get; set; }

        /// <summary>
        ///     Handle to the migration controller.
        /// </summary>
        public MigrationController Migrations { get; set; }
    }
}