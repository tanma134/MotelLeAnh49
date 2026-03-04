
using MotelLeAnh49.Models;

namespace BusinessLogic.Interfaces
{
    public interface IAdminService
    {
        Admin? Login(string username, string password);
        string TestHash(string password);
    }
}