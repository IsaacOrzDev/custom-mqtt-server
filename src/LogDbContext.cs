using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

public class LogDbContext : DbContext
{

  private readonly IConfiguration configuration;
  private readonly string connectionString;

  public LogDbContext(IConfiguration configuration) : base()
  {
    this.configuration = configuration;
    this.connectionString = this.configuration.GetValue<string>("CONNECTION_STRING") ?? "";
  }

  public LogDbContext(DbContextOptions<LogDbContext> options) : base(options)
  {
  }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    if (connectionString is not null)
      optionsBuilder.UseNpgsql(connectionString);
  }

  public DbSet<MessageModel> Messages { get; set; }
  public DbSet<SubscriptionModel> Subscriptions { get; set; }
}