
using System.ComponentModel.DataAnnotations;

public class MessageModel
{
  public int Id { get; set; }
  [Required]
  public required string ClientId { get; set; }
  [Required]
  public required string Topic { get; set; }
  [Required]
  public required string Payload { get; set; }
  public uint ExpiryInterval { get; set; }
  [Required]
  public DateTime ReceivedAt { get; set; }
}