using ProviderInbox.Domain;

namespace ProviderInbox.Service.Features.Inbox.IngestMessage;

public sealed class Request
{
    public string Npi { get; set; } = default!;
    public string OfficeNumber { get; set; } = default!;
    public MessageType Type { get; set; }
    public string Title { get; set; } = default!;
    public string? Summary { get; set; }
    public string? ReferenceId { get; set; }
    public bool ActionRequired { get; set; }
    public DateTimeOffset? DueAt { get; set; }
    public int? SlaHours { get; set; }
    public Guid? Id { get; set; }
}

public sealed class Response
{
    public Guid Id { get; set; }
    public PriorityLevel Priority { get; set; }
    public bool Pinned { get; set; }
}
