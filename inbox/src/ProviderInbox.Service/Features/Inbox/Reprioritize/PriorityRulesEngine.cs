using ProviderInbox.Domain;

namespace ProviderInbox.Service.Features.Inbox.Reprioritize;

public sealed class PriorityRulesEngine
{
    private static readonly TimeSpan P0Window = TimeSpan.FromHours(24);
    private static readonly TimeSpan EscalationWindow = TimeSpan.FromHours(12);

    public bool Apply(InboxMessage m, DateTimeOffset nowUtc)
    {
        var oldPriority = m.Priority;
        var oldPinned = m.Pinned;

        if (m.Status == MessageStatus.Completed)
        {
            m.Priority = PriorityLevel.P3;
            m.Pinned = false;
            return oldPriority != m.Priority || oldPinned != m.Pinned;
        }

        if (!m.ActionRequired)
        {
            m.Priority = PriorityLevel.P2;
            m.Pinned = false;
            return oldPriority != m.Priority || oldPinned != m.Pinned;
        }

        if (m.DueAt is null)
        {
            m.Priority = PriorityLevel.P0;
            m.Pinned = true;
            return oldPriority != m.Priority || oldPinned != m.Pinned;
        }

        var timeToDue = m.DueAt.Value - nowUtc;
        if (timeToDue <= TimeSpan.Zero || timeToDue <= EscalationWindow || timeToDue <= P0Window)
        {
            m.Priority = PriorityLevel.P0;
            m.Pinned = true;
            return oldPriority != m.Priority || oldPinned != m.Pinned;
        }

        m.Priority = PriorityLevel.P1;
        m.Pinned = false;
        return oldPriority != m.Priority || oldPinned != m.Pinned;
    }
}
