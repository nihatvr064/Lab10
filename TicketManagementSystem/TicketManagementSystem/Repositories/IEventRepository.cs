using System;
using System.Collections.Generic;
using System.Text;
using TicketManagementSystem.Models;

namespace TicketManagementSystem.Repositories
{
    public interface IEventRepository
    {
        void Add(Event eventEntity);
        Event GetById(int id);
        List<Event> GetAll();
        void Update(Event eventEntity);
        void Delete(int id);
    }
}
