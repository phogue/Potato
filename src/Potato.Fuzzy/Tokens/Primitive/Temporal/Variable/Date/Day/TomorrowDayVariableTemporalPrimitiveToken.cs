﻿#region Copyright
// Copyright 2015 Geoff Green.
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

namespace Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.Day {
    public class TomorrowDayVariableTemporalPrimitiveToken : DateVariableTemporalPrimitiveToken {
        public new static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<TomorrowDayVariableTemporalPrimitiveToken>(state, phrase);
        }

        public TomorrowDayVariableTemporalPrimitiveToken() {
            var dt = DateTime.Now.AddDays(1);

            Pattern = new FuzzyDateTimePattern() {
                Rule = TimeType.Definitive,
                Year = dt.Year,
                Month = dt.Month,
                Day = dt.Day
            };
        }
    }
}