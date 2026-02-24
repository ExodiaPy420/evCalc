namespace CalculatorService.Core.Exceptions
{
    public class InvalidArgumentsException : BusinessException
    {
        public InvalidArgumentsException(string message) : base("InvalidArguments", message) { }
    }
}
