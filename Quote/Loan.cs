using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quote
{
    public interface Loan
    {
        double LoanAmount { get; set; }
        double Rate { get; set; }
        double MonthlyRepayment { get; set; }
        double TotalRepayment { get; set; }
    }

    public class LoanImpl : Loan
    {
        public double LoanAmount { get; set; }
        public double Rate { get; set; }
        public double MonthlyRepayment { get; set; }
        public double TotalRepayment { get; set; }
             
    }
}
