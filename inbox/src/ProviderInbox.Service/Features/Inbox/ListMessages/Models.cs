using ProviderInbox.Domain;

namespace ProviderInbox.Service.Features.Inbox.ListMessages;

public sealed class Request
{
    public MessageType? Type { get; set; }
    public MessageStatus? Status { get; set; }
    public PriorityLevel? Priority { get; set; }
    public bool? ActionRequired { get; set; }
    public string? Search { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 25;
}

public sealed class Item
{
    public Guid Id { get; set; }
    public MessageType Type { get; set; }
    public string Title { get; set; } = default!;
    public string? Summary { get; set; }
    public string? ReferenceId { get; set; }
    public PriorityLevel Priority { get; set; }
    public bool Pinned { get; set; }
    public bool ActionRequired { get; set; }
    public MessageStatus Status { get; set; }
    public DateTimeOffset? DueAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
