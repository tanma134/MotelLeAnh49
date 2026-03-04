
using MotelLeAnh49.Models;

namespace DataAccess.Repositories.Interfaces
{
    public interface IAdminRepository
    {
        Admin? GetByUsername(string username);
    }
}