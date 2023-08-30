using Application;
using Microsoft.Extensions.Configuration;

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile($"appsettings.json", true, true)
    .AddEnvironmentVariables();
var configuration = builder.Build();

var server = new CustomMQTTServer(configuration);
await server.StartMqttAsync();
Console.WriteLine("MQTT Server is started!");
while (true) ;

