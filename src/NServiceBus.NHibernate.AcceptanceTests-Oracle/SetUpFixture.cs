using System;
using NUnit.Framework;

[SetUpFixture]
public class SetUpFixture
{
    [OneTimeSetUp]
    public void SetUp()
    {
        var connectionString = Environment.GetEnvironmentVariable("OracleConnectionString");
        var ci = Environment.GetEnvironmentVariable("CI");
        if (string.IsNullOrWhiteSpace(connectionString) && ci == "true")
        {
            Assert.Ignore("Ignoring Oracle test");
        }
    }
}
