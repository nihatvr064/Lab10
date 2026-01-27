using TicketManagementSystem.Models;
using TicketManagementSystem.Repositories;

namespace TicketManagementSystem.Services;

public class EventService
{
    private readonly IEventRepository _repo;

    public EventService(IEventRepository repo)
    {
        _repo = repo;
    }

    public void Create(string name, DateTime date, string location)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new Exception("Event adı boş ola bilməz");

        if (string.IsNullOrWhiteSpace(location))
            throw new Exception("Location boş ola bilməz");

        if (date < DateTime.Now)
            throw new Exception("Keçmiş tarixə event yaratmaq olmaz");

        var ev = new Event
        {
            Name = name,
            Date = date,
            Location = location
        };

        _repo.Add(ev);
    }

    public void Delete(int id)
    {
        var ev = _repo.GetById(id);
        if (ev == null)
            throw new Exception("Event tapılmadı");

        _repo.Delete(id);
    }

    public List<Event> GetAll()
    {
        return _repo.GetAll();
    }

    public Event GetById(int id)
    {
        var ev = _repo.GetById(id);
        if (ev == null)
            throw new Exception("Event tapılmadı");

        return ev;
    }

    public void Update(int id, string name, DateTime date, string location)
    {
        var ev = _repo.GetById(id);
        if (ev == null)
            throw new Exception("Event tapılmadı");

        if (string.IsNullOrWhiteSpace(name))
            throw new Exception("Event adı boş ola bilməz");

        if (string.IsNullOrWhiteSpace(location))
            throw new Exception("Location boş ola bilməz");

        if (date < DateTime.Now)
            throw new Exception("Keçmiş tarixə event qoymaq olmaz");

        ev.Name = name;
        ev.Date = date;
        ev.Location = location;

        _repo.Update(ev);
    }
}
