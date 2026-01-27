using TicketManagementSystem.Models;
using TicketManagementSystem.Repositories;

namespace TicketManagementSystem.Services;

public class TicketService
{
    private readonly ITicketRepository _repo;

    public TicketService(ITicketRepository repo)
    {
        _repo = repo;
    }

    public void Reserve(string eventName, string customerName, int seatCount)
    {
        if (seatCount < 1 || seatCount > 5)
            throw new Exception("Seat count 1–5 arası olmalıdır");

        var ticket = new Ticket
        {
            EventName = eventName,
            CustomerName = customerName,
            SeatCount = seatCount,
            Status = TicketStatus.Reserved,
            CreatedAt = DateTime.Now
        };

        _repo.Add(ticket);
    }

    public void Pay(int ticketId)
    {
        var ticket = _repo.GetById(ticketId);
        if (ticket == null)
            throw new Exception("Ticket tapılmadı");

        if (ticket.Status == TicketStatus.Expired || ticket.Status == TicketStatus.Cancelled)
            throw new Exception("Bu ticket dəyişdirilə bilməz");

        if (ticket.Status != TicketStatus.Reserved)
            throw new Exception("Yalnız Reserved ticket ödənilə bilər");

        ticket.Status = TicketStatus.Paid;
        _repo.Update(ticket);
    }

    public void Cancel(int ticketId)
    {
        var ticket = _repo.GetById(ticketId);
        if (ticket == null)
            throw new Exception("Ticket tapılmadı");

        if (ticket.Status == TicketStatus.Expired || ticket.Status == TicketStatus.Cancelled)
            throw new Exception("Bu ticket dəyişdirilə bilməz");

        ticket.Status = TicketStatus.Cancelled;
        _repo.Update(ticket);
    }


    public void ShowStats()
    {
        var tickets = _repo.GetAll();

        Console.WriteLine($"Reserved: {tickets.Count(t => t.Status == TicketStatus.Reserved)}");
        Console.WriteLine($"Paid: {tickets.Count(t => t.Status == TicketStatus.Paid)}");
        Console.WriteLine($"Cancelled: {tickets.Count(t => t.Status == TicketStatus.Cancelled)}");
        Console.WriteLine($"Expired: {tickets.Count(t => t.Status == TicketStatus.Expired)}");

        Console.WriteLine($"Total seats sold: {tickets.Where(t => t.Status == TicketStatus.Paid).Sum(t => t.SeatCount)}");
    }
    public List<Ticket> GetAll()
    {
        return _repo.GetAll();
    }

}
