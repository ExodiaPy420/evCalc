using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculatorService.Core.Exceptions
{
    public class DivisionByZeroException : BusinessException
    {
        public DivisionByZeroException() : base ("DivisionByZero", "Divisor cannot be zero.") { }
    }
}
