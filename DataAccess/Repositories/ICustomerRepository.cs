using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface ICustomerRepository
    {
        IEnumerable<Customer> GetAll();

        void Add(Customer customer);
        void Save();

        Customer GetById(int id);

        void Update(Customer customer);

        void Delete(Customer customer);

        IEnumerable<Customer> SearchByIdentity(string cccd);

        Task<Customer> GetByIdAsync(int id);
    }
}
