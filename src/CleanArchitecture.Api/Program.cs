using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Interfaces;
using Destructurama;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using CleanArchitecture.Infrastructure.Persistence;
using CleanArchitecture.Tools.Postgres;
using Serilog;

namespace CleanArchitecture.Api
{
    public class Program
    {
        private static readonly ManualResetEventSlim Complete = new();
        private static readonly CancellationTokenSource TokenSource = new();

        public static async Task<int> Main(string[] args)
        {
            Activity.DefaultIdFormat = ActivityIdFormat.W3C;

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

            ThreadPool.GetMinThreads(out var defaultWorkerThreads, out var defaultCompletionPortThreads);
            ThreadPool.SetMinThreads(defaultWorkerThreads * 2, 2);

            try
            {
                var host = CreateHostBuilder(args).Build();

                using var scope = host.Services.CreateScope();

                var services = scope.ServiceProvider;

                var context = services.GetRequiredService<IApplicationDbContext>();

                DbMigrationManager.EnsureSeedData(host.Services);

                await ApplicationDbContextSeed.SeedDataAsync(context);

                await host.RunAsync(TokenSource.Token);

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
                Complete.Set();
            }
        }

        public static IWebHostBuilder CreateHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)

                .UseSerilog((context, logConfiguration) => logConfiguration
                    //remove this one line if changing to open telemetry
                    .ReadFrom.Configuration(context.Configuration)
                    .Destructure.UsingAttributes()
                    .Enrich.FromLogContext()
                    //add bottom line for open telemetry
                    //.WriteTo.OpenTelemetry(context.Configuration.GetSection("OpenTelemetry")["Endpoint"] + "/v1/logs", Serilog.Sinks.OpenTelemetry.OtlpProtocol.HttpProtobuf)
                    .WriteTo.Console())
                .UseStartup<Startup>();
        }

        #region Private Methods

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            Complete.Wait(TokenSource.Token);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception exception)
            {
                Log.Error(exception, exception.Message);
            }
        }

        #endregion
    }
}
