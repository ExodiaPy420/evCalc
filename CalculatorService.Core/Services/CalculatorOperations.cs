using CalculatorService.Core.Interfaces;
using CalculatorService.Core.Exceptions;

namespace CalculatorService.Core.Services
{
    public class CalculatorOperations : ICalculatorOperations
    {

        public double Add(IEnumerable<double> addends)
        {
            var list = addends?.ToList();
            if (list == null || list.Count < 2)
                throw new InvalidArgumentsException("At least two operands are required.");

            return list.Sum();
        }

        public double Subtract(double minuend, double subtrahend)
        {
            return minuend - subtrahend;
        }



        public double Multiply(IEnumerable<double> factors) 
        {
            var list = factors?.ToList();
            if (list == null || list.Count < 2)
                throw new InvalidArgumentsException("At least two operands are required.");

            return list.Aggregate(1.0, (acc, x) => acc * x);

        }



        public (double Quotient, double Remainder) Divide(double dividend, double divisor)
        {
            if (divisor == 0) throw new DivisionByZeroException();

            double Quotient = Math.Floor(dividend / divisor);
            double Remainder = dividend % divisor;

            return (Quotient, Remainder);
        }

        public double Sqrt(double number)
        {
            if (number < 0) throw new InvalidArgumentsException("cannot calculate square root of a negative number.");
            return Math.Sqrt(number);
        }


    }
}
