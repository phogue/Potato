using Procon.Database.Serialization.Builders.Methods.Schema;
using Procon.Database.Serialization.Builders.Statements;
using Procon.Database.Shared;

namespace Procon.Database.Serialization.Test {
    public abstract class TestSerializerDrop {
        #region TestDropDatabaseProcon

        protected IDatabaseObject TestDropDatabaseProconExplicit = new Drop()
            .Database(new Builders.Database() {
                Name = "Procon"
            });

        protected IDatabaseObject TestDropDatabaseProconImplicit = new Drop()
            .Database("Procon");

        public abstract void TestDropDatabaseProcon();

        #endregion

        #region TestDropTablePlayer

        protected IDatabaseObject TestDropTablePlayerExplicit = new Drop()
            .Collection(new Collection() {
                Name = "Player"
            });

        protected IDatabaseObject TestDropTablePlayerImplicit = new Drop()
            .Collection("Player");

        public abstract void TestDropTablePlayer();

        #endregion
    }
}
