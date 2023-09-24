using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Trace;
using System.Net;

namespace CleanArchitecture.Api
{
    public class ServerErrorObjectResult
        : ObjectResult
    {
        public ServerErrorObjectResult(object error, HttpStatusCode code)
            : base(error) => StatusCode = (int)code;
    }
}
