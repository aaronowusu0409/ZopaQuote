using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quote
{
    public interface Lender
    {
        string Name { get; }
        double Rate { get; }
        double AvailableFunds { get; } 
    }

    public class LenderImpl : Lender
    {
        public string Name { get; set; }
        public double Rate { get; set; }
        public double AvailableFunds { get; set; }
    }
}
