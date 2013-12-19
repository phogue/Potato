using System.Collections.Generic;
using Procon.Core.Database.Migrations;
using Procon.Net.Utils;

namespace Procon.Core.Test.Database.Helpers {
    public class TestMigrationTrackerHelper {
        /// <summary>
        /// Tracks the last migration called.
        /// </summary>
        public int Tracker { get; set; }

        /// <summary>
        /// Counts how many migrations were called (up or down)
        /// </summary>
        public int Counter { get; set; }

        /// <summary>
        /// Handle to the migration controller.
        /// </summary>
        public MigrationController Migrations { get; set; }

        public TestMigrationTrackerHelper() {
            this.Migrations = new MigrationController() {
                // Bubble all commands to the database controller
                BubbleObjects = new List<IExecutableBase>() {
                    TestDatabaseController.OpenSqLiteDriver()
                },
                Migrations = new List<IMigration>() {
                    new Migration() {
                        Up = () => {
                            this.Tracker = 1;

                            this.Counter++;

                            return true;
                        },
                        Down = () => {
                            this.Tracker = 0;

                            this.Counter++;

                            return true;
                        }
                    },
                    new Migration() {
                        Up = () => {
                            this.Tracker = 2;

                            this.Counter++;

                            return true;
                        },
                        Down = () => {
                            this.Tracker = 1;

                            this.Counter++;

                            return true;
                        }
                    },
                    new Migration() {
                        Up = () => {
                            this.Tracker = 3;

                            this.Counter++;

                            return true;
                        },
                        Down = () => {
                            this.Tracker = 2;

                            this.Counter++;

                            return true;
                        }
                    },
                    new Migration() {
                        Up = () => {
                            this.Tracker = 4;

                            this.Counter++;

                            return true;
                        },
                        Down = () => {
                            this.Tracker = 3;

                            this.Counter++;

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
    }
}
