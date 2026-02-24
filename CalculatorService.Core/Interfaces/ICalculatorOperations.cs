

namespace CalculatorService.Core.Interfaces
{
    public interface ICalculatorOperations
    {

        double Add(IEnumerable<double> addends);

        double Subtract(double minuend, double subtrahend);


        double Multiply(IEnumerable<double> factors);

        (double Quotient, double Remainder)
            Divide(double dividend, double divisor);
        double Sqrt(double number);
    }
}
