using Procon.Database.Serialization.Builders;
using Procon.Database.Serialization.Builders.Attributes;
using Procon.Database.Serialization.Builders.Types;

namespace Procon.Database.Serialization.Test {
    public abstract class TestSerializerDrop {
        #region TestDropDatabaseProcon

        protected IQuery TestDropDatabaseProconExplicit = new Drop()
            .Database(new Builders.Database() {
                Name = "Procon"
            });

        protected IQuery TestDropDatabaseProconImplicit = new Drop()
            .Database("Procon");

        public abstract void TestDropDatabaseProcon();

        #endregion

        #region TestDropTableProcon

        protected IQuery TestDropTableProconExplicit = new Drop()
            .Collection(new Builders.Collection() {
                Name = "Procon"
            });

        protected IQuery TestDropTableProconImplicit = new Drop()
            .Collection("Procon");

        public abstract void TestDropTableProcon();

        #endregion
    }
}
