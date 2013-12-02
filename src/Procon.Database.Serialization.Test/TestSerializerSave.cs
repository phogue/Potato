using Procon.Database.Serialization.Builders;
using Procon.Database.Serialization.Builders.Attributes;
using Procon.Database.Serialization.Builders.Types;

namespace Procon.Database.Serialization.Test {
    public abstract class TestSerializerSave {

        #region TestSaveIntoPlayerSetName

        protected IQuery TestSaveIntoPlayerSetNameExplicit = new Save()
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

        protected IQuery TestSaveIntoPlayerSetNameImplicit = new Save()
            .Collection("Player")
            .Assignment("Name", "Phogue");

        public abstract void TestSaveIntoPlayerSetName();

        #endregion


        #region TestSaveIntoPlayerSetNameScore

        protected IQuery TestSaveIntoPlayerSetNameScoreExplicit = new Save()
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
                    Integer = 50
                }
            });

        protected IQuery TestSaveIntoPlayerSetNameScoreImplicit = new Save()
            .Collection("Player")
            .Assignment("Name", "Phogue")
            .Assignment("Score", 50);

        public abstract void TestSaveIntoPlayerSetNameScore();

        #endregion
    }
}
