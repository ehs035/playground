using System.ComponentModel.DataAnnotations;
namespace ProviderInbox.Domain;

public sealed class InboxMessage
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(10)] public string Npi { get; set; } = default!;
    [Required, MaxLength(20)] public string OfficeNumber { get; set; } = default!;

    public MessageType Type { get; set; }

    [Required, MaxLength(180)] public string Title { get; set; } = default!;
    [MaxLength(400)] public string? Summary { get; set; }
    [MaxLength(64)] public string? ReferenceId { get; set; }

    public bool ActionRequired { get; set; }
    public DateTimeOffset? DueAt { get; set; }
    public int? SlaHours { get; set; }

    public PriorityLevel Priority { get; set; } = PriorityLevel.P2;
    public bool Pinned { get; set; }

    public MessageStatus Status { get; set; } = MessageStatus.Unread;
    public DateTimeOffset? ReadAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }

    public DateTimeOffset? SnoozedUntil { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    [Timestamp] public byte[]? RowVersion { get; set; }
}
