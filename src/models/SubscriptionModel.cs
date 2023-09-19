
using System.ComponentModel.DataAnnotations;

public class SubscriptionModel
{
  public int Id { get; set; }
  [Required]
  public required string ClientId { get; set; }
  [Required]
  public required string Topic { get; set; }
  [Required]
  public DateTime SubscribedAt { get; set; }
}