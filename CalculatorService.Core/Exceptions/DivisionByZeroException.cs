namespace CalculatorService.Core.Exceptions
{
    public class DivisionByZeroException : BusinessException
    {
        public DivisionByZeroException() : base ("DivisionByZero", "Divisor cannot be zero.") { }
    }
}
