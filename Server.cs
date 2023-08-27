using MQTTnet;
using MQTTnet.Server;

namespace Application
{
  public class CustomMQTTServer
  {

    private async Task<string> OnNewMessage(InterceptingPublishEventArgs e)
    {
      Console.WriteLine("Received");
      return "";
    }

    public async Task StartMqttAsync()
    {
      var optionsBuilder = new MqttServerOptionsBuilder()
        .WithConnectionBacklog(100)
        .WithDefaultEndpoint();
      var options = optionsBuilder.Build();
      var mqttServer = new MqttFactory().CreateMqttServer(options);
      mqttServer.InterceptingPublishAsync += this.OnNewMessage;
      // mqttServer.Message
      await mqttServer.StartAsync();
    }
  }
}
