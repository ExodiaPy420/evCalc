namespace CalculatorService.Core.Exceptions
{
    public class BusinessException : Exception
    {
        public string ErrorCode { get; }

        public BusinessException(string errorCode, string message) : base(message) { ErrorCode = errorCode; }
    }
}
