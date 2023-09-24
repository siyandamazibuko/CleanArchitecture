using System;
using CleanArchitecture.Common.Exceptions;

namespace CleanArchitecture.Api.Logging.Destructurers
{
    public class BadRequestExceptionDestructurer : IExceptionDestructurer
    {
        public Type GetTargetType()
        {
            return typeof(BadRequestException);
        }

        public object Handle(Exception ex)
        {
            var validationException = (BadRequestException)ex;

            return new
            {
                validationException.Message
            };
        }
    }
}
