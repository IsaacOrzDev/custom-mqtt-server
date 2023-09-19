using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

public class LogDbContextFactory : IDesignTimeDbContextFactory<LogDbContext>
{
  public LogDbContext CreateDbContext(string[] args)
  {
    IConfigurationRoot configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .AddEnvironmentVariables()
        .Build();

    var builder = new DbContextOptionsBuilder<LogDbContext>();
    var connectionString = configuration.GetValue<String>("CONNECTION_STRING");
    builder.UseNpgsql(connectionString);

    return new LogDbContext(builder.Options);
  }
}