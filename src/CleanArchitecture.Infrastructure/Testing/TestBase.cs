﻿using Destructurama;
using Serilog;

namespace CleanArchitecture.Infrastructure.Testing
{
    public class TestBase
    {
        static TestBase()
        {
            // setup logging configuration
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Destructure.UsingAttributes()
                .WriteTo.Console(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information)
                .CreateLogger();
        }
    }
}
