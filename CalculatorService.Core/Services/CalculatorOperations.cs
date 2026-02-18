using CalculatorService.Core.Interfaces;

namespace CalculatorService.Core.Services
{
    public class CalculatorOperations : ICalculatorOperations
    {

        //public double Add(IEnumerable<double> addends) => addends.Sum();

        public double Add(IEnumerable<double> addends)
        {
            if (addends == null || addends.Count() < 2) //addends.Take(2).Count() < 2???????????
                throw new ArgumentException("At least two operands are required.");

            return addends.Sum();
        }

        public double Subtract(double minuend, double subtrahend) => minuend - subtrahend;

        //public double Multiply(IEnumerable<double> factors) => factors.Aggregate(1.0, (acc, x) => acc * x);

        public double Multiply(IEnumerable<double> factors) 
        {
            if (factors == null || factors.Count() < 2)
                throw new ArgumentException("At least two operands are required.");

            return factors.Aggregate(1.0, (acc, x) => acc * x);

        }



        public (double Quotient, double Remainder) Divide(double dividend, double divisor)
        {
            if (divisor == 0) throw new DivideByZeroException("Can't divide by zero.");



            double Quotient = Math.Floor(dividend / divisor);
            double Remainder = dividend % divisor;

            return (Quotient, Remainder);
            //return Math.Floor(dividend / divisor), dividend % divisor);
        }

        public double Sqrt(double number)
        {
            if (number < 0) throw new ArgumentException("cannot calculate square root of a negative number.");
            return Math.Sqrt(number);
        }


    }
}
