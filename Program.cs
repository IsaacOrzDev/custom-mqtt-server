using Application;

var server = new CustomMQTTServer();
await server.StartMqttAsync();
Console.WriteLine("MQTT Server is started!");
while (true) ;

