using System;
using System.Collections.Generic;
using System.Text;
using TicketManagementSystem.Models;

namespace TicketManagementSystem.Repositories
{
    public interface ITicketRepository
    {
        void Add(Ticket ticket);
        Ticket? GetById(int id);
        void Update(Ticket ticket);
        void Delete(int id);
        List<Ticket> GetAll();
    }
}
