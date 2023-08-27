using Application;

var server = new CustomMQTTServer();
await server.StartMqttAsync();
while (true) ;

