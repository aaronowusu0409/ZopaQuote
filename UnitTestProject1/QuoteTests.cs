using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Quote.Test
{
    [TestClass]
    public class QuoteTests
    {
        static LoanCalculator _loanCalculator;
        static string _errors;
        static Loan _calculatedLoan; 

        [ClassInitialize]
        public static void ClassSetUp(TestContext context)
        {
           Directory.SetCurrentDirectory(@".../.../TestResources/");
        }

        [TestInitialize]
        public void TestSetup()
        {
            _errors = string.Empty;
            _calculatedLoan = null;
            _loanCalculator = new LoanCalculatorImpl();  
        }

        [TestMethod]
        public void LoanCalculator_WhenPassedLoanAmountWhichIsNaN_FailsWithCorrectError()
        {
            _loanCalculator.ParseLendersFromCsvFile("MarketFile.csv", out _errors);
            Assert.IsFalse(_loanCalculator.CalculateLoan("NaN123", out _calculatedLoan, out _errors));
            Assert.AreEqual("Invalid loan amount requested", _errors);
        }

        [TestMethod]
        public void LoanCalculator_WhenPassedMarketFileContainingInvalidLenderEntries_SuccessfullyParsesOnlyValidLenders()
        {
            Assert.IsTrue(_loanCalculator.ParseLendersFromCsvFile("MarketFile_BadlyFormed.csv", out _errors));
            Assert.AreEqual(4, _loanCalculator.Lenders.Length); 
        }

        [TestMethod]
        public void LoanCalculator_WhenValidLoanRequested_SuccessfullyGeneratesLowestPossibleRateLoan()
        {
            _loanCalculator.ParseLendersFromCsvFile("MarketFile.csv", out _errors);
            Assert.IsTrue(_loanCalculator.CalculateLoan("1000", out _calculatedLoan, out _errors));
            Assert.AreEqual(0.06, _calculatedLoan.Rate);
        }

        [TestMethod]
        public void LoanCalculator_WhenValidLoanRequested_SuccessfullyGeneratesCorrectOutput()
        {
            _loanCalculator.ParseLendersFromCsvFile("MarketFile.csv", out _errors);
            Assert.IsTrue(_loanCalculator.CalculateLoan("1200", out _calculatedLoan, out _errors));
            Assert.AreEqual(6.2, Math.Round(_calculatedLoan.Rate * 100, 1));
            Assert.AreEqual(36.59, Math.Round(_calculatedLoan.MonthlyRepayment, 2));
            Assert.AreEqual(1200, _calculatedLoan.LoanAmount);
            Assert.AreEqual(1317.18, Math.Round(_calculatedLoan.TotalRepayment,2));
        }


        [TestMethod]
        public void LoanCalculator_WhenInvalidLoanAmountRequested_FailsWithCorrectError()
        {
            _loanCalculator.ParseLendersFromCsvFile("MarketFile.csv", out _errors);
            Assert.IsFalse(_loanCalculator.CalculateLoan("1111", out _calculatedLoan, out _errors));
            Assert.AreEqual("Invalid loan amount requested", _errors);
        }

        [TestMethod]
        public void LoanCalculator_WhenRequestedLoanAmountExceedsAvailableFunds_FailsWithCorrectError()
        {
            _loanCalculator.ParseLendersFromCsvFile("MarketFile.csv", out _errors);
            Assert.IsFalse(_loanCalculator.CalculateLoan("15000", out _calculatedLoan, out _errors));
            Assert.AreEqual("Unfortuately it is not possible to offer you a quote at this time", _errors); 
        }
    }
}
