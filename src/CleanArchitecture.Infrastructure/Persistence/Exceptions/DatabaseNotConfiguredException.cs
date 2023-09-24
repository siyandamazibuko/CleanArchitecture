using System;

namespace CleanArchitecture.Infrastructure.Persistence.Exceptions
{
    public class DatabaseNotConfiguredException : Exception
    {
        public DatabaseNotConfiguredException() 
        {

        }

        public override string Message => "UseInMemoryDatabase has been set to false in appsettings.This requires you to setup up your DB using a DB technology of your choice ie.Postgres before continuing";
        
    }
}
