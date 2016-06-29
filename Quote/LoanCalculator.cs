using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quote
{
    public interface LoanCalculator
    {
        bool ParseLendersFromCsvFile(string file, out string errors);
        bool CalculateLoan(string requiredAmount, out Loan calculatedParentLoan, out string errors);

        Lender[] Lenders { get; } 
    }

    public class LoanCalculatorImpl : LoanCalculator
    {
        private List<Lender> _lenderList;
        private Loan[] _subLoans;
        private Loan _parentLoan;
        private const int _defaultTermLength = 36;
        private const int _anualRepayments = 12; 
        private int LENDER_NAME = 0; 
        private int LENDER_RATE = 1;
        private int LENDER_AVAILABLE_FUNDS = 2;
        private const int _maxLoanAmount = 15000;
        private const int _minLoanAmount = 1000;

        public LoanCalculatorImpl()
        {
            _lenderList = new List<Lender>();
            _parentLoan = new LoanImpl();
        }

        public bool ParseLendersFromCsvFile(string fileName, out string errors)
        {
            
            errors = string.Empty; 
            if(!File.Exists(fileName))
            {
                errors = "File not found";
                return false; 
            }

            foreach(var line in File.ReadAllLines(fileName))
            {
                var lineArr = line.Split(',');
                if (lineArr.Length != 3) continue;

                double lenderRate;
                double lenderAvialableFunds;

                if (!double.TryParse(lineArr[LENDER_RATE], out lenderRate) ||
                    !double.TryParse(lineArr[LENDER_AVAILABLE_FUNDS], out lenderAvialableFunds))
                    continue;

                _lenderList.Add(new LenderImpl()
                {
                    Name = lineArr[LENDER_NAME],
                    Rate = lenderRate,
                    AvailableFunds = lenderAvialableFunds
                });      
            }

            if (_lenderList.Count > 0)
                return true; 

            errors = "No valid lenders found in market file";
            return false; 
        }

        public bool CalculateLoan(string requiredAmountStr, out Loan calculatedParentLoan, out string errors)
        {
            calculatedParentLoan = null;
            errors = string.Empty;
            double requiredAmount = 0;

            if (!double.TryParse(requiredAmountStr, out requiredAmount)
            || requiredAmount < _minLoanAmount || requiredAmount > _maxLoanAmount
            || (requiredAmount % 100 != 0))
            {
                errors = "Invalid loan amount requested";
                return false;
            }
            
            if (requiredAmount > TotalFundsAvailable)
            {                
                errors = "Unfortuately it is not possible to offer you a quote at this time";
                return false; 
            }

            _subLoans = GenerateSubloans(requiredAmount);
            foreach (var loan in _subLoans)
            {
                _parentLoan.MonthlyRepayment += loan.MonthlyRepayment;  
                _parentLoan.LoanAmount += loan.LoanAmount;
                _parentLoan.TotalRepayment += loan.TotalRepayment; 
            }
            
            _parentLoan.Rate = CalculateAverageRateFromSubLoans(_subLoans, requiredAmount);
            calculatedParentLoan = _parentLoan; 
            return true;  
        }

        private double CalculateMonthlyRepayment(double loanAmount, double rate)
        {
            return loanAmount * ((rate / _anualRepayments) / (1 - Math.Pow((1 + (rate / _anualRepayments)), -1 * _defaultTermLength)));
        }

        private double CalculateTotalRepayment(double monthlyRepayment)
        {
            return monthlyRepayment * _defaultTermLength;
        }

        private double CalculateAverageRateFromSubLoans(Loan[] _subLoans, double requiredAmount)
        {
            double weightedAveRate = 0;
            _subLoans.ToList().ForEach(loan => weightedAveRate += (loan.LoanAmount / requiredAmount) * loan.Rate);
            return weightedAveRate; 
        }

        private double TotalFundsAvailable
        {
            get
            {
                double fundsAvailable = 0;
                _lenderList.ForEach(lender => fundsAvailable += lender.AvailableFunds);
                return fundsAvailable; 
            }
        }

        private Loan[] GenerateSubloans(double requiredAmount)
        {
            var requiredAmountOutstanding = requiredAmount; 
            var sortedLenders = _lenderList.OrderBy(lender => lender.Rate).ToArray();
            var subLoans = new List<Loan>(); 

            for(int i = 0; i < sortedLenders.Count(); i++ )
            {
                if (requiredAmountOutstanding <= 0) break;
                requiredAmountOutstanding -= sortedLenders[i].AvailableFunds;
                var requiredLoanAmount = (requiredAmountOutstanding < 0)
                    ? sortedLenders[i].AvailableFunds + requiredAmountOutstanding
                    : sortedLenders[i].AvailableFunds;
                var calculatedMonthlyRepayment = CalculateMonthlyRepayment(requiredLoanAmount, sortedLenders[i].Rate);
                subLoans.Add(new LoanImpl()
                {
                    LoanAmount = requiredLoanAmount,
                    Rate = sortedLenders[i].Rate,
                    MonthlyRepayment = calculatedMonthlyRepayment,
                    TotalRepayment = CalculateTotalRepayment(calculatedMonthlyRepayment)
                });  
            }

            return subLoans.ToArray();
        }

        public Lender[] Lenders
        {
            get { return _lenderList.ToArray(); } 
        }
    }
}
