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
using System;
using Procon.Database.Shared.Builders.Methods.Data;
using Procon.Database.Shared.Builders.Statements;
using Procon.Database.Shared.Builders.Values;

namespace Procon.Database.Shared.Test {
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
