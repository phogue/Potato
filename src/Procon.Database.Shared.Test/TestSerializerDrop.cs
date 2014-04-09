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
using Procon.Database.Shared.Builders.Methods.Schema;
using Procon.Database.Shared.Builders.Statements;

namespace Procon.Database.Shared.Test {
    public abstract class TestSerializerDrop {
        #region TestDropDatabaseProcon

        protected IDatabaseObject TestDropDatabaseProconExplicit = new Drop()
            .Database(new Shared.Builders.Database() {
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
