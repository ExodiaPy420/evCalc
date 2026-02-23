using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculatorService.Core.Exceptions
{
    public class InvalidArgumentsException : BusinessException
    {
        public InvalidArgumentsException(string message) : base("InvalidArguments", message) { }
    }
}
