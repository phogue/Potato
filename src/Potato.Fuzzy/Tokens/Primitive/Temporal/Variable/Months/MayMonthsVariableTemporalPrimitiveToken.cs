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

namespace Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Months {
    public class MayMonthsVariableTemporalPrimitiveToken : MonthMonthsVariableTemporalPrimitiveToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<MayMonthsVariableTemporalPrimitiveToken>(state, phrase);
        }

        public MayMonthsVariableTemporalPrimitiveToken() {
            Pattern = new FuzzyDateTimePattern() {
                Rule = TimeType.Definitive,
                Year = DateTime.Now.Month <= 5 ? DateTime.Now.Year : DateTime.Now.Year + 1,
                Month = 5
            };
        }
    }
}