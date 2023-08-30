using Microsoft.Extensions.Configuration;
using MQTTnet;
using MQTTnet.Server;

namespace Application
{
  public class CustomMQTTServer
  {
    private IConfiguration configuration;

    public CustomMQTTServer(IConfiguration configuration)
    {
      this.configuration = configuration;
    }

    private Task OnNewMessage(InterceptingPublishEventArgs e)
    {
      Console.WriteLine($"Received Payload: {System.Text.Encoding.Default.GetString(e.ApplicationMessage.PayloadSegment)}");
      Console.WriteLine($"ExpiryInterval: {e.ApplicationMessage.MessageExpiryInterval}");
      Console.WriteLine($"Client ID: {e.ClientId}");
      Console.WriteLine($"Topic: {e.ApplicationMessage.Topic}");
      Console.WriteLine($"Received Time: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
      return Task.CompletedTask;
    }

    private Task OnValidatingConnection(ValidatingConnectionEventArgs e)
    {
      if (!(e.Password == this.configuration.GetValue<string>("PASSWORD") && e.UserName == this.configuration.GetValue<string>("USERNAME")))
      {
        e.ReasonCode = MQTTnet.Protocol.MqttConnectReasonCode.NotAuthorized;
        return Task.FromException(new Exception("User is not authorized"));
      }
      return Task.CompletedTask;
    }

    private Task OnConnected(ClientConnectedEventArgs e)
    {
      Console.WriteLine($"Client ID: {e.ClientId}");
      Console.WriteLine($"Connected Time: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
      return Task.CompletedTask;
    }

    private Task OnDisconnected(ClientDisconnectedEventArgs e)
    {
      Console.WriteLine($"Client ID: {e.ClientId}");
      Console.WriteLine($"Disconnected Time: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
      return Task.CompletedTask;
    }

    public async Task StartMqttAsync()
    {
      var optionsBuilder = new MqttServerOptionsBuilder()
        .WithConnectionBacklog(100)
        .WithDefaultEndpoint();
      var options = optionsBuilder.Build();
      var mqttServer = new MqttFactory().CreateMqttServer(options);
      mqttServer.InterceptingPublishAsync += this.OnNewMessage;
      mqttServer.ValidatingConnectionAsync += this.OnValidatingConnection;
      mqttServer.ClientConnectedAsync += this.OnConnected;
      mqttServer.ClientDisconnectedAsync += this.OnDisconnected;
      await mqttServer.StartAsync();
    }
  }
}
