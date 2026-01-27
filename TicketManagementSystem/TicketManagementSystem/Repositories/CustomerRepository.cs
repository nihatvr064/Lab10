using TicketManagementSystem.Models;

namespace TicketManagementSystem.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context;

    public CustomerRepository(AppDbContext context)
    {
        _context = context;
    }

    public void Add(Customer customer)
    {
        _context.Customers.Add(customer);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var customer = GetById(id);
        if (customer != null)
        {
            _context.Customers.Remove(customer);
            _context.SaveChanges();
        }
    }

    public List<Customer> GetAll()
    {
        return _context.Customers.ToList();
    }

    public Customer GetByEmail(string email)
    {
        return _context.Customers.FirstOrDefault(c => c.Email == email);
    }

    public Customer GetById(int id)
    {
        return _context.Customers.FirstOrDefault(c => c.Id == id);
    }

    public void Update(Customer customer)
    {
        _context.Customers.Update(customer);
        _context.SaveChanges();
    }
}
