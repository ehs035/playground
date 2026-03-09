namespace ProviderInbox.Domain;
public enum MessageType { Claim, Eligibility, Payment, System }
public enum MessageStatus { Unread, Read, Completed }
public enum PriorityLevel { P0 = 0, P1 = 1, P2 = 2, P3 = 3 }
