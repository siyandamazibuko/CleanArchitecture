namespace CleanArchitecture.Common.Exceptions
{
    public class BadRequestException : ValidationException
    {
        public BadRequestException(string message)
            : base(message, null)
        {
        }
    }
}
