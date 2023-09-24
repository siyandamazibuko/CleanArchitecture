using System;
using System.Collections.Generic;

namespace CleanArchitecture.Common.Exceptions
{ 
    public class ValidationException : Exception
    {
        public IDictionary<string, string[]> Errors { get; }

        public ValidationException(ValidationFailureResponse failure)
            : this(failure.Title, failure.Errors)
        {
        }

        public ValidationException(string message, IDictionary<string, string[]> errors)
            : base(message)
        {
            Errors = errors;
        }
    }
}
