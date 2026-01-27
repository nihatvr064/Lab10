using TicketManagementSystem.Models;
using TicketManagementSystem.Repositories;

namespace TicketManagementSystem.Services;

public class CustomerService
{
    private readonly ICustomerRepository _repo;

    public CustomerService(ICustomerRepository repo)
    {
        _repo = repo;
    }

    public void Create(string firstName, string lastName, string email)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new Exception("First name boş ola bilməz");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new Exception("Last name boş ola bilməz");

        if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            throw new Exception("Email düzgün deyil");

        var existing = _repo.GetByEmail(email);
        if (existing != null)
            throw new Exception("Bu email artıq mövcuddur");

        var customer = new Customer
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email
        };

        _repo.Add(customer);
    }

    public void Delete(int id)
    {
        var customer = _repo.GetById(id);
        if (customer == null)
            throw new Exception("Customer tapılmadı");

        _repo.Delete(id);
    }

    public List<Customer> GetAll()
    {
        return _repo.GetAll();
    }

    public Customer GetById(int id)
    {
        var customer = _repo.GetById(id);
        if (customer == null)
            throw new Exception("Customer tapılmadı");

        return customer;
    }

    public void Update(int id, string field, string newValue)
    {
        var customer = _repo.GetById(id);
        if (customer == null)
            throw new Exception("Customer tapılmadı");

        switch (field.ToLower())
        {
            case "firstname":
                if (string.IsNullOrWhiteSpace(newValue))
                    throw new Exception("First name boş ola bilməz");
                customer.FirstName = newValue;
                break;

            case "lastname":
                if (string.IsNullOrWhiteSpace(newValue))
                    throw new Exception("Last name boş ola bilməz");
                customer.LastName = newValue;
                break;

            case "email":
                if (string.IsNullOrWhiteSpace(newValue) || !newValue.Contains("@"))
                    throw new Exception("Email düzgün deyil");

                var existing = _repo.GetByEmail(newValue);
                if (existing != null && existing.Id != id)
                    throw new Exception("Bu email artıq istifadə olunur");

                customer.Email = newValue;
                break;

            default:
                throw new Exception("Yanlış sahə adı. firstname / lastname / email istifadə et");
        }

        _repo.Update(customer);
    }
}
