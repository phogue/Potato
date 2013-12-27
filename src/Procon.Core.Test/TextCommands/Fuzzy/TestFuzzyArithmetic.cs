#region

using NUnit.Framework;

#endregion

namespace Procon.Core.Test.TextCommands.Fuzzy {
    [TestFixture]
    public class TestFuzzyArithmetic : TestTextCommandParserBase {
        [Test]
        public void TestCalculateBasicArithmeticAddition() {
            AssertNumericCommand(CreateTextCommandController(), "calculate 1 + 2", TextCommandCalculate, 3.0F);
        }

        [Test]
        public void TestCalculateBasicArithmeticAdditionNoSpaces() {
            AssertNumericCommand(CreateTextCommandController(), "calculate 1+2", TextCommandCalculate, 3.0F);
        }

        [Test]
        public void TestCalculateBasicArithmeticBodmasAdditionLeftMultiplicationRight() {
            AssertNumericCommand(CreateTextCommandController(), "calculate 1 + 2 * 3", TextCommandCalculate, 7.0F);
        }

        [Test]
        public void TestCalculateBasicArithmeticBodmasAdditionLeftMultiplicationRightNoSpaces() {
            AssertNumericCommand(CreateTextCommandController(), "calculate 1+2*3", TextCommandCalculate, 7.0F);
        }

        [Test]
        public void TestCalculateBasicArithmeticBodmasAdditionRightMultiplicationLeft() {
            AssertNumericCommand(CreateTextCommandController(), "calculate 1 * 2 + 3", TextCommandCalculate, 5.0F);
        }

        [Test]
        public void TestCalculateBasicArithmeticBodmasAdditionRightMultiplicationLeftNoSpaces() {
            AssertNumericCommand(CreateTextCommandController(), "calculate 1*2+3", TextCommandCalculate, 5.0F);
        }

        [Test]
        public void TestCalculateBasicArithmeticDivisionOperator() {
            AssertNumericCommand(CreateTextCommandController(), "calculate 1 / 2", TextCommandCalculate, 0.5F);
        }

        [Test]
        public void TestCalculateBasicArithmeticDivisionOperatorDivisionByZero() {
            AssertNumericCommand(CreateTextCommandController(), "calculate 1 / 0", TextCommandCalculate, 0.0F);
        }

        [Test]
        public void TestCalculateBasicArithmeticDivisionWord() {
            AssertNumericCommand(CreateTextCommandController(), "calculate 1 divide by 2", TextCommandCalculate, 0.5F);
        }

        [Test]
        public void TestCalculateBasicArithmeticMultiplicationOperator() {
            AssertNumericCommand(CreateTextCommandController(), "calculate 1 * 2", TextCommandCalculate, 2.0F);
        }

        [Test]
        public void TestCalculateBasicArithmeticMultiplicationWord() {
            AssertNumericCommand(CreateTextCommandController(), "calculate 1 multiplied by 2", TextCommandCalculate, 2.0F);
        }

        [Test]
        public void TestCalculateBasicArithmeticPower() {
            AssertNumericCommand(CreateTextCommandController(), "calculate 3 ^ 2", TextCommandCalculate, 9.0F);
        }

        [Test]
        public void TestCalculateBasicArithmeticSubtractionOperator() {
            AssertNumericCommand(CreateTextCommandController(), "calculate 1 - 2", TextCommandCalculate, -1.0F);
        }

        [Test]
        public void TestCalculateBasicArithmeticSubtractionWord() {
            AssertNumericCommand(CreateTextCommandController(), "calculate 1 minus 2", TextCommandCalculate, -1.0F);
        }

        [Test]
        public void TestCalculateBracketedArithmeticAdditionNoSpaces() {
            AssertNumericCommand(CreateTextCommandController(), "calculate (1+2)", TextCommandCalculate, 3.0F);
        }

        [Test]
        public void TestCalculateBracketedArithmeticBodmasAdditionLeftMultiplicationRightNoSpaces() {
            AssertNumericCommand(CreateTextCommandController(), "calculate (1+2)*3", TextCommandCalculate, 9.0F);
        }

        [Test]
        public void TestCalculateBracketedArithmeticBodmasAdditionRightMultiplicationLeftNoSpaces() {
            AssertNumericCommand(CreateTextCommandController(), "calculate 1*(2+3)", TextCommandCalculate, 5.0F);
        }

        [Test]
        public void TestCalculateCardinals() {
            AssertNumericCommand(CreateTextCommandController(), "calculate one", TextCommandCalculate, 1.0F);
        }

        [Test]
        public void TestCalculateCardinalsAddition() {
            AssertNumericCommand(CreateTextCommandController(), "calculate twenty one + one hundred and one", TextCommandCalculate, 122.0F);
        }

        [Test]
        public void TestCalculateCardinalsExtendedOneImpliedAdditionHundreds() {
            AssertNumericCommand(CreateTextCommandController(), "calculate one hundred twenty one", TextCommandCalculate, 121.0F);
        }

        [Test]
        public void TestCalculateCardinalsExtendedTwoImpliedAdditionHundreds() {
            AssertNumericCommand(CreateTextCommandController(), "calculate two hundred twenty one", TextCommandCalculate, 221.0F);
        }

        [Test]
        public void TestCalculateCardinalsImpliedAdditionHundreds() {
            AssertNumericCommand(CreateTextCommandController(), "calculate hundred twenty one", TextCommandCalculate, 121.0F);
        }

        [Test]
        public void TestCalculateCardinalsImpliedAdditionTens() {
            AssertNumericCommand(CreateTextCommandController(), "calculate twenty one", TextCommandCalculate, 21.0F);
        }

        [Test]
        public void TestCalculateComplexBracketedArithmeticBodmasLevelOne() {
            AssertNumericCommand(CreateTextCommandController(), "calculate 6*((2+3*3)/2)", TextCommandCalculate, 33.0F);
        }

        [Test]
        public void TestCalculateComplexBracketedArithmeticBodmasLevelThree() {
            AssertNumericCommand(CreateTextCommandController(), "calculate (51+3)/6*((2+3*3)/2)*(51+3)/6*((2+3*3)/2)", TextCommandCalculate, 2450.25F);
        }

        [Test]
        public void TestCalculateComplexBracketedArithmeticBodmasLevelTwo() {
            AssertNumericCommand(CreateTextCommandController(), "calculate (51+3)/6*((2+3*3)/2)", TextCommandCalculate, 49.5F);
        }

        [Test]
        public void TestCalculateComplexBracketedArithmeticBodmasPowerFunction() {
            AssertNumericCommand(CreateTextCommandController(), "calculate 3 pow(2+1)", TextCommandCalculate, 27.0F);
        }
    }
}