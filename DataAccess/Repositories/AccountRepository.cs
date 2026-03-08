using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;
using MotelLeAnh49.Data;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly MotelDbContext _context;

        public AccountRepository(MotelDbContext context)
        {
            _context = context;
        }

        public Account? GetByUsername(string username)
        {
            return _context.Accounts
                .Include(a => a.Customer)
                .FirstOrDefault(a => a.Username == username);
        }

        public Account? GetByEmail(string email)
        {
            return _context.Accounts
                .FirstOrDefault(a => a.Email == email);
        }

        public void Add(Account account)
        {
            _context.Accounts.Add(account);
        }

        public void Update(Account account)
        {
            _context.Accounts.Update(account);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public Account GetById(int id)
        {
            // Tìm tài khoản theo Id, trả về null nếu không thấy
            return _context.Accounts.FirstOrDefault(a => a.Id == id);
        }
    }
}
