using TicketManagementSystem.Models;

namespace TicketManagementSystem.Repositories;

public class TicketRepository : ITicketRepository
{
    private readonly AppDbContext _context;

    public TicketRepository(AppDbContext context)
    {
        _context = context;
    }

    public void Add(Ticket ticket)
    {
        _context.Tickets.Add(ticket);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var ticket = GetById(id);
        if (ticket != null)
        {
            _context.Tickets.Remove(ticket);
            _context.SaveChanges();
        }
    }

    public List<Ticket> GetAll()
    {
        return _context.Tickets.ToList();
    }

    public Ticket GetById(int id)
    {
        return _context.Tickets.FirstOrDefault(t => t.Id == id);
    }

    public void Update(Ticket ticket)
    {
        _context.Tickets.Update(ticket);
        _context.SaveChanges();
    }

}
