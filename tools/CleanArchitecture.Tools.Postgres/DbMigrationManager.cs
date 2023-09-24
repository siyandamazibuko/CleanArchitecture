using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CleanArchitecture.Domain.Entities;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CleanArchitecture.Infrastructure.Persistence;
using CsvHelper.Configuration;
using Serilog;
using System.Net.Mail;

namespace CleanArchitecture.Tools.Postgres
{
    public static class DbMigrationManager
    {
        public static void EnsureSeedData(IServiceProvider serviceProvider, bool delete = false, bool test = false)
        {
            using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
            context.Database.SetCommandTimeout(300);

            if (delete)
            {
                context.Database.EnsureDeleted();
            }

            var migrations = context.Database.GetPendingMigrations();
            migrations.ToList().ForEach(Console.WriteLine);
            context.Database.Migrate();

            if (test)
            {
                EnsureTestData(context);
            }
        }

        #region Private Methods                

        private static Stream GetCsvFileStream(string name)
        {
            var assembly = typeof(Startup).Assembly;
            var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Resources.{name}");
            return stream;
        }

        private static void EnsureTestData(ApplicationDbContext context)
        {
            if (context.Users.Any()) return;
            var users = new List<User>();
            using (var stream = GetCsvFileStream("users.csv"))
            {
                Log.Information("Loading users into database");
                
                TextReader textReader = new StreamReader(stream);
                var csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = "|",
                    HasHeaderRecord = true
                };
                var csv = new CsvReader(textReader, csvConfiguration);

                csv.Read();
                csv.ReadHeader();

                while (csv.Read())
                {
                    var id = csv.GetField<Guid>(0);
                    var name = csv.GetField<string>(1);
                    var surname = csv.GetField<string>(2);
                    var dateOfBirth = csv.GetField<DateTime>(3);
                    var emailAddress = csv.GetField<string>(4);
                    var telephone = csv.GetField<string>(5);

                    var user = new User
                    {
                        Id = id,
                        Name = name,
                        Telephone = telephone,
                        EmailAddress = emailAddress,
                        Surname = surname,
                        DateOfBirth = dateOfBirth
                    };
                    users.Add(user);
                }

                textReader.Close();
            }

            foreach (var country in context.Users)
            {
                context.Users.Remove(country);
            }

            context.Users.AddRange(users);
            context.SaveChanges();
        }

        #endregion
    }
}
