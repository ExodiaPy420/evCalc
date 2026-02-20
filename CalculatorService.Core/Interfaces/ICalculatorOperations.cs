

namespace CalculatorService.Core.Interfaces
{
    public interface ICalculatorOperations
    {

        uint Add(IEnumerable<uint> addends);

        //double Subtract(double minuend, double subtrahend);
        double Subtract(double minuend, IEnumerable<double> subtrahends);


        double Multiply(IEnumerable<double> factors);

        (double Quotient, double Remainder)
            Divide(double dividend, double divisor);
        double Sqrt(double number);
    }
}
