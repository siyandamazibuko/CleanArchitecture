using System;

namespace CleanArchitecture.Api.Logging.Destructurers
{
    public interface IExceptionDestructurer
    {
        Type GetTargetType();

        object Handle(Exception ex);
    }
}
