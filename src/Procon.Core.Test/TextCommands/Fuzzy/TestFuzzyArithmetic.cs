using NUnit.Framework;

namespace Procon.Core.Test.TextCommands.Fuzzy {
    [TestFixture]
    public class TestFuzzyArithmetic : TestFuzzyBase {

        [Test]
        public void TestCalculateBasicArithmeticAddition() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate 1 + 2", TestFuzzyBase.TextCommandCalculate, 3.0F); 
        }

        [Test]
        public void TestCalculateBasicArithmeticSubtractionOperator() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate 1 - 2", TestFuzzyBase.TextCommandCalculate, -1.0F);
        }

        [Test]
        public void TestCalculateBasicArithmeticSubtractionWord() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate 1 minus 2", TestFuzzyBase.TextCommandCalculate, -1.0F);
        }

        [Test]
        public void TestCalculateBasicArithmeticMultiplicationOperator() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate 1 * 2", TestFuzzyBase.TextCommandCalculate, 2.0F);
        }

        [Test]
        public void TestCalculateBasicArithmeticMultiplicationWord() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate 1 multiplied by 2", TestFuzzyBase.TextCommandCalculate, 2.0F);
        }

        [Test]
        public void TestCalculateBasicArithmeticDivisionOperator() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate 1 / 2", TestFuzzyBase.TextCommandCalculate, 0.5F);
        }

        [Test]
        public void TestCalculateBasicArithmeticDivisionWord() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate 1 divide by 2", TestFuzzyBase.TextCommandCalculate, 0.5F);
        }

        [Test]
        public void TestCalculateBasicArithmeticDivisionOperatorDivisionByZero() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate 1 / 0", TestFuzzyBase.TextCommandCalculate, 0.0F);
        }

        [Test]
        public void TestCalculateBasicArithmeticPower() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate 3 ^ 2", TestFuzzyBase.TextCommandCalculate, 9.0F);
        }

        [Test]
        public void TestCalculateBasicArithmeticBodmasAdditionLeftMultiplicationRight() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate 1 + 2 * 3", TestFuzzyBase.TextCommandCalculate, 7.0F);
        }

        [Test]
        public void TestCalculateBasicArithmeticBodmasAdditionRightMultiplicationLeft() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate 1 * 2 + 3", TestFuzzyBase.TextCommandCalculate, 5.0F);
        }

        [Test]
        public void TestCalculateCardinals() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate one", TestFuzzyBase.TextCommandCalculate, 1.0F);
        }

        [Test]
        public void TestCalculateCardinalsImpliedAdditionTens() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate twenty one", TestFuzzyBase.TextCommandCalculate, 21.0F);
        }

        [Test]
        public void TestCalculateCardinalsImpliedAdditionHundreds() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate hundred twenty one", TestFuzzyBase.TextCommandCalculate, 121.0F);
        }

        [Test]
        public void TestCalculateCardinalsExtendedOneImpliedAdditionHundreds() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate one hundred twenty one", TestFuzzyBase.TextCommandCalculate, 121.0F);
        }

        [Test]
        public void TestCalculateCardinalsExtendedTwoImpliedAdditionHundreds() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate two hundred twenty one", TestFuzzyBase.TextCommandCalculate, 221.0F);
        }

        [Test]
        public void TestCalculateCardinalsAddition() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate twenty one + one hundred and one", TestFuzzyBase.TextCommandCalculate, 122.0F);
        }

        [Test]
        public void TestCalculateBasicArithmeticAdditionNoSpaces() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate 1+2", TestFuzzyBase.TextCommandCalculate, 3.0F);
        }

        [Test]
        public void TestCalculateBasicArithmeticBodmasAdditionLeftMultiplicationRightNoSpaces() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate 1+2*3", TestFuzzyBase.TextCommandCalculate, 7.0F);
        }

        [Test]
        public void TestCalculateBasicArithmeticBodmasAdditionRightMultiplicationLeftNoSpaces() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate 1*2+3", TestFuzzyBase.TextCommandCalculate, 5.0F);
        }

        [Test]
        public void TestCalculateBracketedArithmeticAdditionNoSpaces() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate (1+2)", TestFuzzyBase.TextCommandCalculate, 3.0F);
        }

        [Test]
        public void TestCalculateBracketedArithmeticBodmasAdditionLeftMultiplicationRightNoSpaces() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate (1+2)*3", TestFuzzyBase.TextCommandCalculate, 9.0F);
        }

        [Test]
        public void TestCalculateBracketedArithmeticBodmasAdditionRightMultiplicationLeftNoSpaces() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate 1*(2+3)", TestFuzzyBase.TextCommandCalculate, 5.0F);
        }

        [Test]
        public void TestCalculateComplexBracketedArithmeticBodmasLevelOne() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate 6*((2+3*3)/2)", TestFuzzyBase.TextCommandCalculate, 33.0F);
        }

        [Test]
        public void TestCalculateComplexBracketedArithmeticBodmasLevelTwo() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate (51+3)/6*((2+3*3)/2)", TestFuzzyBase.TextCommandCalculate, 49.5F);
        }

        [Test]
        public void TestCalculateComplexBracketedArithmeticBodmasLevelThree() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate (51+3)/6*((2+3*3)/2)*(51+3)/6*((2+3*3)/2)", TestFuzzyBase.TextCommandCalculate, 2450.25F);
        }

        [Test]
        public void TestCalculateComplexBracketedArithmeticBodmasPowerFunction() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate 3 pow(2+1)", TestFuzzyBase.TextCommandCalculate, 27.0F);
        }
    }
}
