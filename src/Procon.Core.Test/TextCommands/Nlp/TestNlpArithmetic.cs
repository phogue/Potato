using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Procon.Core.Test.TextCommands.Nlp {
    [TestClass]
    public class TestNlpArithmetic : TestNlpBase {

        [TestMethod]
        public void TestCalculateBasicArithmeticAddition() {
            TestNlpBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate 1 + 2", TestNlpBase.TextCommandCalculate, 3.0F); 
        }

        [TestMethod]
        public void TestCalculateBasicArithmeticSubtractionOperator() {
            TestNlpBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate 1 - 2", TestNlpBase.TextCommandCalculate, -1.0F);
        }

        [TestMethod]
        public void TestCalculateBasicArithmeticSubtractionWord() {
            TestNlpBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate 1 minus 2", TestNlpBase.TextCommandCalculate, -1.0F);
        }

        [TestMethod]
        public void TestCalculateBasicArithmeticMultiplicationOperator() {
            TestNlpBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate 1 * 2", TestNlpBase.TextCommandCalculate, 2.0F);
        }

        [TestMethod]
        public void TestCalculateBasicArithmeticMultiplicationWord() {
            TestNlpBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate 1 multiplied by 2", TestNlpBase.TextCommandCalculate, 2.0F);
        }

        [TestMethod]
        public void TestCalculateBasicArithmeticDivisionOperator() {
            TestNlpBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate 1 / 2", TestNlpBase.TextCommandCalculate, 0.5F);
        }

        [TestMethod]
        public void TestCalculateBasicArithmeticDivisionWord() {
            TestNlpBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate 1 divide by 2", TestNlpBase.TextCommandCalculate, 0.5F);
        }

        [TestMethod]
        public void TestCalculateBasicArithmeticDivisionOperatorDivisionByZero() {
            TestNlpBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate 1 / 0", TestNlpBase.TextCommandCalculate, 0.0F);
        }

        [TestMethod]
        public void TestCalculateBasicArithmeticPower() {
            TestNlpBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate 3 ^ 2", TestNlpBase.TextCommandCalculate, 9.0F);
        }

        [TestMethod]
        public void TestCalculateBasicArithmeticBodmasAdditionLeftMultiplicationRight() {
            TestNlpBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate 1 + 2 * 3", TestNlpBase.TextCommandCalculate, 7.0F);
        }

        [TestMethod]
        public void TestCalculateBasicArithmeticBodmasAdditionRightMultiplicationLeft() {
            TestNlpBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate 1 * 2 + 3", TestNlpBase.TextCommandCalculate, 5.0F);
        }

        [TestMethod]
        public void TestCalculateCardinals() {
            TestNlpBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate one", TestNlpBase.TextCommandCalculate, 1.0F);
        }

        [TestMethod]
        public void TestCalculateCardinalsImpliedAdditionTens() {
            TestNlpBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate twenty one", TestNlpBase.TextCommandCalculate, 21.0F);
        }

        [TestMethod]
        public void TestCalculateCardinalsImpliedAdditionHundreds() {
            TestNlpBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate hundred twenty one", TestNlpBase.TextCommandCalculate, 121.0F);
        }

        [TestMethod]
        public void TestCalculateCardinalsExtendedOneImpliedAdditionHundreds() {
            TestNlpBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate one hundred twenty one", TestNlpBase.TextCommandCalculate, 121.0F);
        }

        [TestMethod]
        public void TestCalculateCardinalsExtendedTwoImpliedAdditionHundreds() {
            TestNlpBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate two hundred twenty one", TestNlpBase.TextCommandCalculate, 221.0F);
        }

        [TestMethod]
        public void TestCalculateCardinalsAddition() {
            TestNlpBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate twenty one + one hundred and one", TestNlpBase.TextCommandCalculate, 122.0F);
        }

        [TestMethod]
        public void TestCalculateBasicArithmeticAdditionNoSpaces() {
            TestNlpBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate 1+2", TestNlpBase.TextCommandCalculate, 3.0F);
        }

        [TestMethod]
        public void TestCalculateBasicArithmeticBodmasAdditionLeftMultiplicationRightNoSpaces() {
            TestNlpBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate 1+2*3", TestNlpBase.TextCommandCalculate, 7.0F);
        }

        [TestMethod]
        public void TestCalculateBasicArithmeticBodmasAdditionRightMultiplicationLeftNoSpaces() {
            TestNlpBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate 1*2+3", TestNlpBase.TextCommandCalculate, 5.0F);
        }

        [TestMethod]
        public void TestCalculateBracketedArithmeticAdditionNoSpaces() {
            TestNlpBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate (1+2)", TestNlpBase.TextCommandCalculate, 3.0F);
        }

        [TestMethod]
        public void TestCalculateBracketedArithmeticBodmasAdditionLeftMultiplicationRightNoSpaces() {
            TestNlpBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate (1+2)*3", TestNlpBase.TextCommandCalculate, 9.0F);
        }

        [TestMethod]
        public void TestCalculateBracketedArithmeticBodmasAdditionRightMultiplicationLeftNoSpaces() {
            TestNlpBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate 1*(2+3)", TestNlpBase.TextCommandCalculate, 5.0F);
        }

        [TestMethod]
        public void TestCalculateComplexBracketedArithmeticBodmasLevelOne() {
            TestNlpBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate 6*((2+3*3)/2)", TestNlpBase.TextCommandCalculate, 33.0F);
        }

        [TestMethod]
        public void TestCalculateComplexBracketedArithmeticBodmasLevelTwo() {
            TestNlpBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate (51+3)/6*((2+3*3)/2)", TestNlpBase.TextCommandCalculate, 49.5F);
        }

        [TestMethod]
        public void TestCalculateComplexBracketedArithmeticBodmasLevelThree() {
            TestNlpBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate (51+3)/6*((2+3*3)/2)*(51+3)/6*((2+3*3)/2)", TestNlpBase.TextCommandCalculate, 2450.25F);
        }

        [TestMethod]
        public void TestCalculateComplexBracketedArithmeticBodmasPowerFunction() {
            TestNlpBase.AssertNumericCommand(this.CreateTextCommandController(), "calculate 3 pow(2+1)", TestNlpBase.TextCommandCalculate, 27.0F);
        }
    }
}
