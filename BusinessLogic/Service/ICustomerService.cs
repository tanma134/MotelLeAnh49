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
        Customer GetProfile(int id);
        void DeleteCustomer(int customerId);

        IEnumerable<Customer> SearchByIdentity(string cccd);

        //  Kiểm tra email/phone trùng lặp
        bool IsEmailDuplicate(string email, int customerId);
        bool IsPhoneDuplicate(string phone, int customerId);

    }
}
