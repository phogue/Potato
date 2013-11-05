using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Procon.Core.Test.TextCommands.Fuzzy {
    [TestClass]
    public class TestFuzzyArithmetic : TestFuzzyBase {

        [TestMethod]
        public void TestCalculateBasicArithmeticAddition() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate 1 + 2", TestFuzzyBase.TextCommandCalculate, 3.0F); 
        }

        [TestMethod]
        public void TestCalculateBasicArithmeticSubtractionOperator() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate 1 - 2", TestFuzzyBase.TextCommandCalculate, -1.0F);
        }

        [TestMethod]
        public void TestCalculateBasicArithmeticSubtractionWord() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate 1 minus 2", TestFuzzyBase.TextCommandCalculate, -1.0F);
        }

        [TestMethod]
        public void TestCalculateBasicArithmeticMultiplicationOperator() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate 1 * 2", TestFuzzyBase.TextCommandCalculate, 2.0F);
        }

        [TestMethod]
        public void TestCalculateBasicArithmeticMultiplicationWord() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate 1 multiplied by 2", TestFuzzyBase.TextCommandCalculate, 2.0F);
        }

        [TestMethod]
        public void TestCalculateBasicArithmeticDivisionOperator() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate 1 / 2", TestFuzzyBase.TextCommandCalculate, 0.5F);
        }

        [TestMethod]
        public void TestCalculateBasicArithmeticDivisionWord() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate 1 divide by 2", TestFuzzyBase.TextCommandCalculate, 0.5F);
        }

        [TestMethod]
        public void TestCalculateBasicArithmeticDivisionOperatorDivisionByZero() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate 1 / 0", TestFuzzyBase.TextCommandCalculate, 0.0F);
        }

        [TestMethod]
        public void TestCalculateBasicArithmeticPower() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate 3 ^ 2", TestFuzzyBase.TextCommandCalculate, 9.0F);
        }

        [TestMethod]
        public void TestCalculateBasicArithmeticBodmasAdditionLeftMultiplicationRight() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate 1 + 2 * 3", TestFuzzyBase.TextCommandCalculate, 7.0F);
        }

        [TestMethod]
        public void TestCalculateBasicArithmeticBodmasAdditionRightMultiplicationLeft() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate 1 * 2 + 3", TestFuzzyBase.TextCommandCalculate, 5.0F);
        }

        [TestMethod]
        public void TestCalculateCardinals() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate one", TestFuzzyBase.TextCommandCalculate, 1.0F);
        }

        [TestMethod]
        public void TestCalculateCardinalsImpliedAdditionTens() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate twenty one", TestFuzzyBase.TextCommandCalculate, 21.0F);
        }

        [TestMethod]
        public void TestCalculateCardinalsImpliedAdditionHundreds() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate hundred twenty one", TestFuzzyBase.TextCommandCalculate, 121.0F);
        }

        [TestMethod]
        public void TestCalculateCardinalsExtendedOneImpliedAdditionHundreds() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate one hundred twenty one", TestFuzzyBase.TextCommandCalculate, 121.0F);
        }

        [TestMethod]
        public void TestCalculateCardinalsExtendedTwoImpliedAdditionHundreds() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate two hundred twenty one", TestFuzzyBase.TextCommandCalculate, 221.0F);
        }

        [TestMethod]
        public void TestCalculateCardinalsAddition() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate twenty one + one hundred and one", TestFuzzyBase.TextCommandCalculate, 122.0F);
        }

        [TestMethod]
        public void TestCalculateBasicArithmeticAdditionNoSpaces() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate 1+2", TestFuzzyBase.TextCommandCalculate, 3.0F);
        }

        [TestMethod]
        public void TestCalculateBasicArithmeticBodmasAdditionLeftMultiplicationRightNoSpaces() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate 1+2*3", TestFuzzyBase.TextCommandCalculate, 7.0F);
        }

        [TestMethod]
        public void TestCalculateBasicArithmeticBodmasAdditionRightMultiplicationLeftNoSpaces() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate 1*2+3", TestFuzzyBase.TextCommandCalculate, 5.0F);
        }

        [TestMethod]
        public void TestCalculateBracketedArithmeticAdditionNoSpaces() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate (1+2)", TestFuzzyBase.TextCommandCalculate, 3.0F);
        }

        [TestMethod]
        public void TestCalculateBracketedArithmeticBodmasAdditionLeftMultiplicationRightNoSpaces() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate (1+2)*3", TestFuzzyBase.TextCommandCalculate, 9.0F);
        }

        [TestMethod]
        public void TestCalculateBracketedArithmeticBodmasAdditionRightMultiplicationLeftNoSpaces() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate 1*(2+3)", TestFuzzyBase.TextCommandCalculate, 5.0F);
        }

        [TestMethod]
        public void TestCalculateComplexBracketedArithmeticBodmasLevelOne() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate 6*((2+3*3)/2)", TestFuzzyBase.TextCommandCalculate, 33.0F);
        }

        [TestMethod]
        public void TestCalculateComplexBracketedArithmeticBodmasLevelTwo() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate (51+3)/6*((2+3*3)/2)", TestFuzzyBase.TextCommandCalculate, 49.5F);
        }

        [TestMethod]
        public void TestCalculateComplexBracketedArithmeticBodmasLevelThree() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate (51+3)/6*((2+3*3)/2)*(51+3)/6*((2+3*3)/2)", TestFuzzyBase.TextCommandCalculate, 2450.25F);
        }

        [TestMethod]
        public void TestCalculateComplexBracketedArithmeticBodmasPowerFunction() {
            TestFuzzyBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate 3 pow(2+1)", TestFuzzyBase.TextCommandCalculate, 27.0F);
        }
    }
}
