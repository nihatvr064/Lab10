using TicketManagementSystem.Models;

namespace TicketManagementSystem.Repositories;

public class EventRepository : IEventRepository
{
    private readonly AppDbContext _context;

    public EventRepository(AppDbContext context)
    {
        _context = context;
    }

    public void Add(Event eventEntity)
    {
        _context.Events.Add(eventEntity);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var ev = GetById(id);
        if (ev != null)
        {
            _context.Events.Remove(ev);
            _context.SaveChanges();
        }
    }

    public List<Event> GetAll()
    {
        return _context.Events.ToList();
    }

    public Event GetById(int id)
    {
        return _context.Events.FirstOrDefault(e => e.Id == id);
    }

    public void Update(Event eventEntity)
    {
        _context.Events.Update(eventEntity);
        _context.SaveChanges();
    }
}
