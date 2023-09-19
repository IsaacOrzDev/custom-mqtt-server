using Microsoft.Extensions.Configuration;
using MQTTnet;
using MQTTnet.Server;

namespace Application
{
  public class MQTTServer
  {
    private readonly IConfiguration configuration;
    private readonly LogDbContext dbContext;

    private readonly bool isDatabaseConnected = false;

    public MQTTServer(IConfiguration configuration, LogDbContext dbContext)
    {
      this.configuration = configuration;
      this.dbContext = dbContext;
      this.isDatabaseConnected = this.dbContext.Database.CanConnect();
    }

    private async Task OnNewMessage(InterceptingPublishEventArgs e)
    {
      var payload = System.Text.Encoding.Default.GetString(e.ApplicationMessage.PayloadSegment);

      if (isDatabaseConnected)
      {
        try
        {
          dbContext.Add(new MessageModel
          {
            Payload = payload,
            ClientId = e.ClientId,
            Topic = e.ApplicationMessage.Topic,
            ExpiryInterval = e.ApplicationMessage.MessageExpiryInterval,
            ReceivedAt = DateTime.Now.ToUniversalTime()
          });
          await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
          Console.WriteLine($"An error occurred: {ex}");
        }
      }
      else
      {
        Console.WriteLine($"Received Payload: {payload}");
        Console.WriteLine($"ExpiryInterval: {e.ApplicationMessage.MessageExpiryInterval}");
        Console.WriteLine($"Client ID: {e.ClientId}");
        Console.WriteLine($"Topic: {e.ApplicationMessage.Topic}");
        Console.WriteLine($"Received Time: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
      }
    }

    private Task OnValidatingConnection(ValidatingConnectionEventArgs e)
    {
      if (!(e.Password == configuration.GetValue<string>("PASSWORD") && e.UserName == configuration.GetValue<string>("USERNAME")))
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

    public async Task OnTopicSubscribed(ClientSubscribedTopicEventArgs e)
    {
      if (isDatabaseConnected)
      {
        try
        {
          dbContext.Subscriptions.Add(
            new SubscriptionModel
            {
              ClientId = e.ClientId,
              Topic = e.TopicFilter.Topic,
              SubscribedAt = DateTime.Now.ToUniversalTime()
            }
          );
          await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
          Console.WriteLine($"An error occurred: {ex}");
        }
      }
      else
      {
        Console.WriteLine($"Client ID: {e.ClientId}");
        Console.WriteLine($"Subscribed Topic: {e.TopicFilter.Topic}");
      }
    }

    public async Task StartMqttAsync()
    {
      var optionsBuilder = new MqttServerOptionsBuilder()
        .WithConnectionBacklog(100)
        .WithDefaultEndpoint();
      var options = optionsBuilder.Build();
      var mqttServer = new MqttFactory().CreateMqttServer(options);
      mqttServer.InterceptingPublishAsync += OnNewMessage;
      mqttServer.ValidatingConnectionAsync += OnValidatingConnection;
      mqttServer.ClientConnectedAsync += OnConnected;
      mqttServer.ClientDisconnectedAsync += OnDisconnected;
      mqttServer.ClientSubscribedTopicAsync += OnTopicSubscribed;
      await mqttServer.StartAsync();
    }
  }
}
