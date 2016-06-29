using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quote
{
    class Program
    {
        static void Main(string[] args)
        {
            int MARKET_FILE = 0;
            int REQUIRED_LOAN_AMOUNT = 1;
            string errors = string.Empty;

            var _loanCalculator = new LoanCalculatorImpl();
            Loan calculatedLoan = null; 

            if (args.Length != 2)
            {
                Console.WriteLine("Program invoked with incorrect number of arguments");
                Console.ReadLine(); 
                return; 
            }

            if (!_loanCalculator.ParseLendersFromCsvFile(args[MARKET_FILE], out errors))
            {
                Console.WriteLine("Unable to parse Market CSV file - " + errors);
                Console.ReadLine();
                return;
            } 

            if(!_loanCalculator.CalculateLoan(args[REQUIRED_LOAN_AMOUNT], out calculatedLoan, out errors))
            {
                Console.WriteLine(errors);
                Console.ReadLine();
                return; 
            }

            var outputResult = string.Format(
                "Requested amount: £{0}" +
                "\nRate: {1}%" +
                "\nMonthly repayment: £{2} " +
                "\nTotal repayment: £{3}", calculatedLoan.LoanAmount,
                Math.Round((calculatedLoan.Rate * 100), 1), Math.Round(calculatedLoan.MonthlyRepayment,2),
                Math.Round(calculatedLoan.TotalRepayment,2));

            Console.WriteLine(outputResult);
            Console.ReadLine();
        }
    }
}
