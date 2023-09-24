using System;

namespace CleanArchitecture.Common.Extensions
{
    public static class StringExtensions
    {
        public static Guid ToGuid(this string value)
        {
            Guid.TryParse(value, out var result);
            return result;
        }
    }
}


