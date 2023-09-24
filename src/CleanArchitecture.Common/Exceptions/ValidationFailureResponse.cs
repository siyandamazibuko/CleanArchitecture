using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Common.Exceptions
{
    public class ValidationFailureResponse
    {
        public string Title { get; set; }

        public IDictionary<string, string[]> Errors { get; set; }
    }
}
