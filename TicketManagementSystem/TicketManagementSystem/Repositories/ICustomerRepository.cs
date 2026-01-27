using System;
using System.Collections.Generic;
using System.Text;
using TicketManagementSystem.Models;

namespace TicketManagementSystem.Repositories
{
    public interface ICustomerRepository
    {
        Customer GetById(int id);
        Customer GetByEmail(string email);
        void Add(Customer customer);
        void Delete(int id);
        List<Customer> GetAll();
        void Update(Customer customer);
    }
}
