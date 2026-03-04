
using DataAccess.Repositories.Interfaces;
using MotelLeAnh49.Data;
using MotelLeAnh49.Models;

namespace DataAccess.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly MotelDbContext _context;

        public AdminRepository(MotelDbContext context)
        {
            _context = context;
        }

        public Admin? GetByUsername(string username)
        {
            return _context.Admins
                .FirstOrDefault(a => a.Username == username && a.IsActive);
        }
    }
}