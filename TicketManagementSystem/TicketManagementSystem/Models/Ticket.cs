namespace TicketManagementSystem.Models;

public enum TicketStatus
{
    Reserved,
    Paid,
    Cancelled,
    Expired
}

public class Ticket : Entity
{
    public string EventName { get; set; }
    public string CustomerName { get; set; }
    public int SeatCount { get; set; } 
    public TicketStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
