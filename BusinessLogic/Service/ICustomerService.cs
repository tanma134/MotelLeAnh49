using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;

namespace BusinessLogic.Service
{
    public interface ICustomerService
    {
        IEnumerable<Customer> GetAllCustomers();

        void CreateCustomer(Customer customer, Account account);

        Customer GetCustomerById(int id);

        void UpdateCustomer(Customer customer);

        void DeleteCustomer(int customerId);
    }
}
