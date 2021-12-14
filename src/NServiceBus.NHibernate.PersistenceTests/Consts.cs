namespace NServiceBus.NHibernate.PersistenceTests
{
    using System;

    static class Consts
    {
        const string defaultConnStr = @"Data Source=.\SQLEXPRESS;Initial Catalog=nservicebus;Integrated Security=True;";

        public static string ConnectionString
        {
            get
            {
                var env = Environment.GetEnvironmentVariable("SQLServerConnectionString");
                return string.IsNullOrEmpty(env) ? defaultConnStr : env;
            }
        }
    }
}