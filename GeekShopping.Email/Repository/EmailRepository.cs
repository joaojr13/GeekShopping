using GeekShopping.Email.Messages;
using GeekShopping.Email.Models;
using GeekShopping.Email.Models.Context;
using GeekShopping.Email.Repository;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.CartAPI.Repository
{
    public class EmailRepository : IEmailRepository
    {
        private readonly DbContextOptions<MySQLContext> _context;

        public EmailRepository(DbContextOptions<MySQLContext> context)
        {
            _context = context;
        }
        public async Task SendAndLogEmail(UpdatePaymentResultMessage message)
        {
            EmailLog email = new()
            {
                Email = message.Email,
                SentDate = DateTime.Now,
                Log = $"Order - {message.OrderId} has been created successfully!!!"
            };

            await using var _db = new MySQLContext(_context);
            _db.Emails.Add(email);
            await _db.SaveChangesAsync();
        }
    }
}
