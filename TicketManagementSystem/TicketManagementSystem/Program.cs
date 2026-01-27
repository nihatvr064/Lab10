using Microsoft.EntityFrameworkCore;
using TicketManagementSystem.Repositories;
using TicketManagementSystem.Services;
using System.Text;

namespace TicketManagementSystem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            using var context = new AppDbContext();

            // Ensure database is created
            context.Database.EnsureCreated();

            // Repositories
            var customerRepo = new CustomerRepository(context);
            var eventRepo = new EventRepository(context);
            var ticketRepo = new TicketRepository(context);

            // Services
            var customerService = new CustomerService(customerRepo);
            var eventService = new EventService(eventRepo);
            var ticketService = new TicketService(ticketRepo);

            var app = new TicketManagementApp(customerService, eventService, ticketService);
            app.Run();
        }
    }

    public class TicketManagementApp
    {
        private readonly CustomerService _customerService;
        private readonly EventService _eventService;
        private readonly TicketService _ticketService;

        public TicketManagementApp(CustomerService customerService, EventService eventService, TicketService ticketService)
        {
            _customerService = customerService;
            _eventService = eventService;
            _ticketService = ticketService;
        }

        public void Run()
        {
            while (true)
            {
                try
                {
                    DisplayMenu();
                    var choice = GetInput("Seçim edin");

                    if (!ProcessChoice(choice))
                        break;
                }
                catch (Exception ex)
                {
                    DisplayError($"Xəta: {ex.Message}");
                    if (ex.InnerException != null)
                        DisplayError($"Ətraflı: {ex.InnerException.Message}");
                }

                Console.WriteLine("\nDavam etmək üçün Enter basın...");
                Console.ReadLine();
            }
        }

        private void DisplayMenu()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔═════════════════════════════════════════════════════════╗");
            Console.WriteLine("║       TICKET MANAGEMENT SYSTEM - BILET SİSTEMİ          ║");
            Console.WriteLine("╚═════════════════════════════════════════════════════════╝");
            Console.ResetColor();

            Console.WriteLine("\n┌─── MÜŞTƏRİ İDARƏSİ ───┐");
            Console.WriteLine("│ 1  - Müştəri yarat     │");
            Console.WriteLine("│ 2  - Müştəriləri göstər│");
            Console.WriteLine("│ 3  - Müştəri yenilə    │");
            Console.WriteLine("│ 4  - Müştəri sil       │");
            Console.WriteLine("└────────────────────────┘");

            Console.WriteLine("\n┌─── TƏDBİR İDARƏSİ ────┐");
            Console.WriteLine("│ 5  - Tədbir yarat      │");
            Console.WriteLine("│ 6  - Təbirləri göstər  │");
            Console.WriteLine("│ 7  - Tədbir yenilə     │");
            Console.WriteLine("│ 8  - Tədbir sil        │");
            Console.WriteLine("└────────────────────────┘");

            Console.WriteLine("\n┌─── BİLET İDARƏSİ ─────┐");
            Console.WriteLine("│ 9  - Bilet rezerv et   │");
            Console.WriteLine("│ 10 - Bilet ödə         │");
            Console.WriteLine("│ 11 - Bilet ləğv et     │");
            Console.WriteLine("│ 12 - Biletləri göstər  │");
            Console.WriteLine("│ 13 - Statistika        │");
            Console.WriteLine("│ 14 - Müştəri biletləri │");
            Console.WriteLine("│ 15 - Tədbir biletləri  │");
            Console.WriteLine("└────────────────────────┘");

            Console.WriteLine("\n┌────────────────────────┐");
            Console.WriteLine("│ 0  - Çıxış             │");
            Console.WriteLine("└────────────────────────┘\n");
        }

        private bool ProcessChoice(string choice)
        {
            switch (choice)
            {
                case "1": CreateCustomer(); break;
                case "2": ListCustomers(); break;
                case "3": UpdateCustomer(); break;
                case "4": DeleteCustomer(); break;
                case "5": CreateEvent(); break;
                case "6": ListEvents(); break;
                case "7": UpdateEvent(); break;
                case "8": DeleteEvent(); break;
                case "9": ReserveTicket(); break;
                case "10": PayTicket(); break;
                case "11": CancelTicket(); break;
                case "12": ListAllTickets(); break;
                case "13": ShowStatistics(); break;
                case "14": ShowCustomerTickets(); break;
                case "15": ShowEventTickets(); break;
                case "0":
                    Console.WriteLine("\nSistem bağlanır...");
                    return false;
                default:
                    DisplayWarning("Yanlış seçim! Zəhmət olmasa 0-15 arası rəqəm seçin.");
                    break;
            }
            return true;
        }

        #region Customer Operations

        private void CreateCustomer()
        {
            Console.WriteLine("\n=== YENİ MÜŞTƏRİ ===");
            var firstName = GetInput("Ad");
            var lastName = GetInput("Soyad");
            var email = GetInput("Email");

            if (!IsValidEmail(email))
            {
                DisplayWarning("Email düzgün formatda deyil!");
                return;
            }

            _customerService.Create(firstName, lastName, email);
            DisplaySuccess("✓ Müştəri uğurla yaradıldı!");
        }

        private void ListCustomers()
        {
            Console.WriteLine("\n=== MÜŞTƏRİLƏR SİYAHISI ===");
            var customers = _customerService.GetAll();

            if (!customers.Any())
            {
                DisplayWarning("Heç bir müştəri tapılmadı.");
                return;
            }

            Console.WriteLine($"\n{"ID",-5} {"AD",-15} {"SOYAD",-15} {"EMAIL",-30}");
            Console.WriteLine(new string('-', 70));

            foreach (var customer in customers)
            {
                Console.WriteLine($"{customer.Id,-5} {customer.FirstName,-15} {customer.LastName,-15} {customer.Email,-30}");
            }

            Console.WriteLine($"\nCəmi: {customers.Count()} müştəri");
        }

        private void UpdateCustomer()
        {
            Console.WriteLine("\n=== MÜŞTƏRİ YENİLƏ ===");
            var id = GetIntInput("Müştəri ID");

            Console.WriteLine("\nDəyişdirmək istədiyiniz sahəni seçin:");
            Console.WriteLine("1 - Ad (FirstName)");
            Console.WriteLine("2 - Soyad (LastName)");
            Console.WriteLine("3 - Email");

            var fieldChoice = GetInput("Seçim");
            string field = fieldChoice switch
            {
                "1" => "firstname",
                "2" => "lastname",
                "3" => "email",
                _ => GetInput("Sahə adı (firstname, lastname, email)")
            };

            var value = GetInput("Yeni dəyər");

            if (field.ToLower() == "email" && !IsValidEmail(value))
            {
                DisplayWarning("Email düzgün formatda deyil!");
                return;
            }

            _customerService.Update(id, field, value);
            DisplaySuccess("✓ Müştəri məlumatları yeniləndi!");
        }

        private void DeleteCustomer()
        {
            Console.WriteLine("\n=== MÜŞTƏRİ SİL ===");
            var id = GetIntInput("Müştəri ID");

            Console.Write("Əminsiniz? (b/x): ");
            if (Console.ReadLine()?.ToLower() == "b")
            {
                _customerService.Delete(id);
                DisplaySuccess("✓ Müştəri silindi!");
            }
            else
            {
                Console.WriteLine("Əməliyyat ləğv edildi.");
            }
        }

        #endregion

        #region Event Operations

        private void CreateEvent()
        {
            Console.WriteLine("\n=== YENİ TƏDBİR ===");
            var name = GetInput("Tədbir adı");
            var date = GetDateInput("Tarix (yyyy-MM-dd)");
            var location = GetInput("Məkan");

            if (date < DateTime.Now.Date)
            {
                DisplayWarning("Keçmiş tarixə tədbir yarada bilməzsiniz!");
                return;
            }

            _eventService.Create(name, date, location);
            DisplaySuccess("✓ Tədbir uğurla yaradıldı!");
        }

        private void ListEvents()
        {
            Console.WriteLine("\n=== TƏDBİRLƏR SİYAHISI ===");
            var events = _eventService.GetAll();

            if (!events.Any())
            {
                DisplayWarning("Heç bir tədbir tapılmadı.");
                return;
            }

            Console.WriteLine($"\n{"ID",-5} {"AD",-25} {"TARİX",-15} {"MƏKAN",-20}");
            Console.WriteLine(new string('-', 70));

            foreach (var ev in events.OrderBy(e => e.Date))
            {
                var dateStr = ev.Date.ToString("dd.MM.yyyy");
                Console.WriteLine($"{ev.Id,-5} {ev.Name,-25} {dateStr,-15} {ev.Location,-20}");
            }

            Console.WriteLine($"\nCəmi: {events.Count()} tədbir");
        }

        private void UpdateEvent()
        {
            Console.WriteLine("\n=== TƏDBİR YENİLƏ ===");
            var id = GetIntInput("Tədbir ID");
            var name = GetInput("Yeni ad");
            var date = GetDateInput("Yeni tarix (yyyy-MM-dd)");
            var location = GetInput("Yeni məkan");

            if (date < DateTime.Now.Date)
            {
                DisplayWarning("Keçmiş tarixə tədbir təyin edə bilməzsiniz!");
                return;
            }

            _eventService.Update(id, name, date, location);
            DisplaySuccess("✓ Tədbir məlumatları yeniləndi!");
        }

        private void DeleteEvent()
        {
            Console.WriteLine("\n=== TƏDBİR SİL ===");
            var id = GetIntInput("Tədbir ID");

            Console.Write("Əminsiniz? Bu tədbirin bütün biletləri də silinəcək! (b/x): ");
            if (Console.ReadLine()?.ToLower() == "b")
            {
                _eventService.Delete(id);
                DisplaySuccess("✓ Tədbir silindi!");
            }
            else
            {
                Console.WriteLine("Əməliyyat ləğv edildi.");
            }
        }

        #endregion

        #region Ticket Operations

        private void ReserveTicket()
        {
            Console.WriteLine("\n=== BİLET REZERV ET ===");

            // Show available events
            var events = _eventService.GetAll();
            if (!events.Any())
            {
                DisplayWarning("Heç bir tədbir mövcud deyil!");
                return;
            }

            Console.WriteLine("\nMövcud tədbirlər:");
            foreach (var ev in events)
                Console.WriteLine($"  {ev.Id}. {ev.Name} - {ev.Date:dd.MM.yyyy}");

            var eventName = GetInput("\nTədbir adı");

            // Show customers
            var customers = _customerService.GetAll();
            if (!customers.Any())
            {
                DisplayWarning("Heç bir müştəri mövcud deyil!");
                return;
            }

            Console.WriteLine("\nMövcud müştərilər:");
            foreach (var c in customers)
                Console.WriteLine($"  {c.Id}. {c.FirstName} {c.LastName}");

            var customerName = GetInput("\nMüştəri adı (Ad Soyad)");
            var seatCount = GetIntInput("Yer sayı");

            if (seatCount <= 0)
            {
                DisplayWarning("Yer sayı müsbət olmalıdır!");
                return;
            }

            _ticketService.Reserve(eventName, customerName, seatCount);
            DisplaySuccess($"✓ {seatCount} yer üçün bilet rezerv edildi!");
        }

        private void PayTicket()
        {
            Console.WriteLine("\n=== BİLET ÖDƏ ===");
            var id = GetIntInput("Bilet ID");

            _ticketService.Pay(id);
            DisplaySuccess("✓ Bilet ödənildi!");
        }

        private void CancelTicket()
        {
            Console.WriteLine("\n=== BİLET LƏĞV ET ===");
            var id = GetIntInput("Bilet ID");

            Console.Write("Əminsiniz? (b/x): ");
            if (Console.ReadLine()?.ToLower() == "b")
            {
                _ticketService.Cancel(id);
                DisplaySuccess("✓ Bilet ləğv edildi!");
            }
            else
            {
                Console.WriteLine("Əməliyyat ləğv edildi.");
            }
        }

        private void ListAllTickets()
        {
            Console.WriteLine("\n=== BÜTÜN BİLETLƏR ===");
            var tickets = _ticketService.GetAll();

            if (!tickets.Any())
            {
                DisplayWarning("Heç bir bilet tapılmadı.");
                return;
            }

            Console.WriteLine($"\n{"ID",-5} {"TƏDBİR",-20} {"MÜŞTƏRİ",-20} {"YER",-5} {"STATUS",-12} {"TARİX",-12}");
            Console.WriteLine(new string('-', 85));

            foreach (var ticket in tickets.OrderByDescending(t => t.CreatedAt))
            {
                var status = GetStatusDisplay(ticket.Status);
                var date = ticket.CreatedAt.ToString("dd.MM.yyyy");
                Console.WriteLine($"{ticket.Id,-5} {TruncateString(ticket.EventName, 20),-20} {TruncateString(ticket.CustomerName, 20),-20} " +
                                $"{ticket.SeatCount,-5} {status,-12} {date,-12}");
            }

            Console.WriteLine($"\nCəmi: {tickets.Count()} bilet");
        }

        private void ShowStatistics()
        {
            Console.WriteLine("\n=== STATİSTİKA ===");
            _ticketService.ShowStats();
        }

        private void ShowCustomerTickets()
        {
            Console.WriteLine("\n=== MÜŞTƏRİ BİLETLƏRİ ===");
            var customerName = GetInput("Müştəri adı (Ad Soyad)");

            var tickets = _ticketService.GetAll()
                .Where(t => t.CustomerName.Contains(customerName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!tickets.Any())
            {
                DisplayWarning($"{customerName} üçün bilet tapılmadı.");
                return;
            }

            Console.WriteLine($"\n{"ID",-5} {"TƏDBİR",-25} {"YER",-5} {"STATUS",-12} {"TARİX",-12}");
            Console.WriteLine(new string('-', 70));

            foreach (var ticket in tickets.OrderByDescending(t => t.CreatedAt))
            {
                var status = GetStatusDisplay(ticket.Status);
                var date = ticket.CreatedAt.ToString("dd.MM.yyyy");
                Console.WriteLine($"{ticket.Id,-5} {TruncateString(ticket.EventName, 25),-25} " +
                                $"{ticket.SeatCount,-5} {status,-12} {date,-12}");
            }

            Console.WriteLine($"\nCəmi: {tickets.Count} bilet");
        }

        private void ShowEventTickets()
        {
            Console.WriteLine("\n=== TƏDBİR BİLETLƏRİ ===");
            var eventName = GetInput("Tədbir adı");

            var tickets = _ticketService.GetAll()
                .Where(t => t.EventName.Contains(eventName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!tickets.Any())
            {
                DisplayWarning($"{eventName} üçün bilet tapılmadı.");
                return;
            }

            Console.WriteLine($"\n{"ID",-5} {"MÜŞTƏRİ",-25} {"YER",-5} {"STATUS",-12} {"TARİX",-12}");
            Console.WriteLine(new string('-', 70));

            foreach (var ticket in tickets.OrderByDescending(t => t.CreatedAt))
            {
                var status = GetStatusDisplay(ticket.Status);
                var date = ticket.CreatedAt.ToString("dd.MM.yyyy");
                Console.WriteLine($"{ticket.Id,-5} {TruncateString(ticket.CustomerName, 25),-25} " +
                                $"{ticket.SeatCount,-5} {status,-12} {date,-12}");
            }

            var totalSeats = tickets.Sum(t => t.SeatCount);
            Console.WriteLine($"\nCəmi: {tickets.Count} bilet, {totalSeats} yer");
        }

        #endregion

        #region Helper Methods

        private string GetInput(string prompt)
        {
            Console.Write($"{prompt}: ");
            var input = Console.ReadLine()?.Trim();

            while (string.IsNullOrWhiteSpace(input))
            {
                DisplayWarning("Bu sahə boş ola bilməz!");
                Console.Write($"{prompt}: ");
                input = Console.ReadLine()?.Trim();
            }

            return input;
        }

        private int GetIntInput(string prompt)
        {
            while (true)
            {
                Console.Write($"{prompt}: ");
                if (int.TryParse(Console.ReadLine(), out int result))
                    return result;

                DisplayWarning("Zəhmət olmasa düzgün rəqəm daxil edin!");
            }
        }

        private DateTime GetDateInput(string prompt)
        {
            while (true)
            {
                Console.Write($"{prompt}: ");
                if (DateTime.TryParse(Console.ReadLine(), out DateTime result))
                    return result;

                DisplayWarning("Zəhmət olmasa düzgün tarix formatı daxil edin (yyyy-MM-dd)!");
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email && email.Contains("@");
            }
            catch
            {
                return false;
            }
        }

        private string GetStatusDisplay(object status)
        {
            if (status == null) return "N/A";

            var statusStr = status.ToString();
            return statusStr?.ToLower() switch
            {
                "reserved" => "Rezerv",
                "paid" => "Ödənilib",
                "cancelled" => "Ləğv",
                _ => statusStr ?? "N/A"
            };
        }

        private string TruncateString(string str, int maxLength)
        {
            if (string.IsNullOrEmpty(str)) return "";
            return str.Length <= maxLength ? str : str.Substring(0, maxLength - 3) + "...";
        }

        private void DisplaySuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        private void DisplayWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"⚠ {message}");
            Console.ResetColor();
        }

        private void DisplayError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"✗ {message}");
            Console.ResetColor();
        }

        #endregion
    }
}