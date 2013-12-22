using System;
using Procon.Database.Shared;
using Procon.Database.Shared.Builders.Methods.Data;
using Procon.Database.Shared.Builders.Statements;
using Procon.Database.Shared.Builders.Values;

namespace Procon.Database.Serialization.Test {
    public abstract class TestSerializerSave {

        #region TestSaveIntoPlayerSetName

        protected IDatabaseObject TestSaveIntoPlayerSetNameExplicit = new Save()
            .Collection(new Collection() {
                Name = "Player"
            })
            .Set(new Assignment() {
                new Field() {
                    Name = "Name"
                },
                new StringValue() {
                    Data = "Phogue"
                }
            });

        protected IDatabaseObject TestSaveIntoPlayerSetNameImplicit = new Save()
            .Collection("Player")
            .Set("Name", "Phogue");

        public abstract void TestSaveIntoPlayerSetName();

        #endregion

        #region TestSaveIntoPlayerSetNameScore

        protected IDatabaseObject TestSaveIntoPlayerSetNameScoreExplicit = new Save()
            .Collection(new Collection() {
                Name = "Player"
            })
            .Set(new Assignment() {
                new Field() {
                    Name = "Name"
                },
                new StringValue() {
                    Data = "Phogue"
                }
            })
            .Set(new Assignment() {
                new Field() {
                    Name = "Score"
                },
                new NumericValue() {
                    Long = 50
                }
            });

        protected IDatabaseObject TestSaveIntoPlayerSetNameScoreImplicit = new Save()
            .Collection("Player")
            .Set("Name", "Phogue")
            .Set("Score", 50);

        public abstract void TestSaveIntoPlayerSetNameScore();

        #endregion

        #region TestSaveIntoPlayerSetNameAndStamp

        protected IDatabaseObject TestSaveIntoPlayerSetNameAndStampExplicit = new Save()
            .Collection(new Collection() {
                Name = "Player"
            })
            .Set(new Assignment() {
                new Field() {
                    Name = "Name"
                },
                new StringValue() {
                    Data = "Phogue"
                }
            })
            .Set(new Assignment() {
                new Field() {
                    Name = "Stamp"
                },
                new DateTimeValue() {
                    Data = new DateTime(2013,12, 19, 1, 8, 0, 55)
                }
            });

        protected IDatabaseObject TestSaveIntoPlayerSetNameAndStampImplicit = new Save()
            .Collection("Player")
            .Set("Name", "Phogue")
            .Set("Stamp", new DateTime(2013, 12, 19, 1, 8, 0, 55));

        public abstract void TestSaveIntoPlayerSetNameAndStamp();

        #endregion

    }
}
