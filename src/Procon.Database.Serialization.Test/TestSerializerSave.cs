using Procon.Database.Serialization.Builders;
using Procon.Database.Serialization.Builders.Methods;
using Procon.Database.Serialization.Builders.Statements;
using Procon.Database.Serialization.Builders.Values;

namespace Procon.Database.Serialization.Test {
    public abstract class TestSerializerSave {

        #region TestSaveIntoPlayerSetName

        protected IDatabaseObject TestSaveIntoPlayerSetNameExplicit = new Save()
            .Collection(new Collection() {
                Name = "Player"
            })
            .Assignment(new Assignment() {
                new Field() {
                    Name = "Name"
                },
                new StringValue() {
                    Data = "Phogue"
                }
            });

        protected IDatabaseObject TestSaveIntoPlayerSetNameImplicit = new Save()
            .Collection("Player")
            .Assignment("Name", "Phogue");

        public abstract void TestSaveIntoPlayerSetName();

        #endregion

        #region TestSaveIntoPlayerSetNameScore

        protected IDatabaseObject TestSaveIntoPlayerSetNameScoreExplicit = new Save()
            .Collection(new Collection() {
                Name = "Player"
            })
            .Assignment(new Assignment() {
                new Field() {
                    Name = "Name"
                },
                new StringValue() {
                    Data = "Phogue"
                }
            })
            .Assignment(new Assignment() {
                new Field() {
                    Name = "Score"
                },
                new NumericValue() {
                    Long = 50
                }
            });

        protected IDatabaseObject TestSaveIntoPlayerSetNameScoreImplicit = new Save()
            .Collection("Player")
            .Assignment("Name", "Phogue")
            .Assignment("Score", 50);

        public abstract void TestSaveIntoPlayerSetNameScore();

        #endregion
    }
}
