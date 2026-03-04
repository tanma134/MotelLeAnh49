using BusinessLogic.Interfaces;
using DataAccess.Repositories.Interfaces;
using MotelLeAnh49.Models;
using System.Security.Cryptography;
using System.Text;

namespace BusinessLogic.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _repo;

        public AdminService(IAdminRepository repo)
        {
            _repo = repo;
        }

        public Admin? Login(string username, string password)
        {
            var admin = _repo.GetByUsername(username);
            if (admin == null) return null;

            string hashedInput = HashPassword(password);

            if (admin.Password != hashedInput)
                return null;

            return admin;
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
        public string TestHash(string password)
        {
            return HashPassword(password);
        }
    }
}