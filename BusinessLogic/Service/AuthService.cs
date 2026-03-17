using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;
using DataAccess.Repositories;
using MotelLeAnh49.Data;

namespace BusinessLogic.Service
{
    public class AuthService
    {
        private readonly IAccountRepository _repo;
        private readonly MotelDbContext _context;
        public AuthService(IAccountRepository repo, MotelDbContext context)
        {
            _repo = repo;
            _context = context;
        }

        public bool IsUsernameExist(string username)
        {
            return _repo.GetByUsername(username) != null;
        }

        public bool IsEmailExist(string email)
        {
            return _repo.GetByEmail(email) != null;
        }

        public Account? Login(string username, string password)
        {
            var account = _repo.GetByUsername(username);

            if (account == null)
                return null;

            if (account.Password != HashPassword(password))
                return null;

            return account;
        }


        public bool Register(Account account, Customer customer)
        {
            if (IsUsernameExist(account.Username))
                return false;

            if (IsEmailExist(account.Email))
                return false;

            account.Password = HashPassword(account.Password);
            account.Role = "Customer";
            account.IsActive = false;

            _repo.Add(account);
            _repo.Save();

            customer.AccountId = account.Id;
            _context.Customers.Add(customer);
            _context.SaveChanges();

            return true;
        }
        public Account GetByEmail(string email)
        {
            return _repo.GetByEmail(email);
        }
        public void ResetPassword(string email, string newPassword)
        {
            var account = _repo.GetByEmail(email);

            account.Password = HashPassword(newPassword);

            _repo.Update(account);
            _repo.Save();
        }
        public string GenerateOTP()
        {
            Random rand = new Random();
            return rand.Next(100000, 999999).ToString();
        }

        public string HashPassword(string password)
        {
            using SHA256 sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
        public void ActivateAccount(string email)
        {
            var account = _repo.GetByEmail(email);

            if (account != null)
            {
                account.IsActive = true;
                _repo.Update(account);
                _repo.Save();
            }
        }
    }
}
