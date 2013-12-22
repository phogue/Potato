#region

using System.Collections.Generic;
using Procon.Core.Shared;
using Procon.Core.Shared.Database.Migrations;
using Procon.Net.Shared.Utils;
using Procon.Net.Utils;

#endregion

namespace Procon.Core.Test.Database.Helpers {
    public class TestMigrationTrackerHelper {
        public TestMigrationTrackerHelper() {
            Migrations = new MigrationController() {
                // Bubble all commands to the database controller
                BubbleObjects = new List<IExecutableBase>() {
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