using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface IAccountRepository
    {
        Account? GetByUsername(string username);

        Account? GetByEmail(string email);

        void Add(Account account);

        void Update(Account account);

        void Save();
    }
}

