using Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile($"appsettings.json", true, true)
    .AddEnvironmentVariables();



var configuration = builder.Build();

var provider = new ServiceCollection()
    .AddSingleton<IConfiguration>(configuration)
    .AddTransient<LogDbContext, LogDbContext>()
    .AddTransient<MQTTServer, MQTTServer>()
    .BuildServiceProvider();

;
// using (var dbContext = provider.GetService<LogDbContext>())
// {
//     if (dbContext is not null)
//     {
//         if (!await dbContext.Database.EnsureCreatedAsync())
//         {
//             await dbContext.Database.MigrateAsync();
//         }
//     }
// }

var server = provider.GetService<MQTTServer>();
if (server is null)
    return;
else
{
    await server.StartMqttAsync();
    Console.WriteLine("MQTT Server is started!");
    while (true) ;

}
