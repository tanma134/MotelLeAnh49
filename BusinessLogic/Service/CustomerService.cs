using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using MotelLeAnh49.Data;

namespace BusinessLogic.Service
{
    public class CustomerService : ICustomerService
    {

        private readonly ICustomerRepository _customerRepo;
        private readonly IAccountRepository _accountRepo;

        public CustomerService(ICustomerRepository customerRepo, IAccountRepository accountRepo)
        {
            _customerRepo = customerRepo;
            _accountRepo = accountRepo;
        }

        public IEnumerable<Customer> GetAllCustomers()
        {
            var allCustomers = _customerRepo.GetAll();

            return allCustomers.Where(c => c.Account != null && c.Account.IsActive == true);
        }

        public void CreateCustomer(Customer customer, Account account)
        {
            // Bước 1: Lưu Account trước để DB sinh ra cái Id tự tăng
            _accountRepo.Add(account);
            _accountRepo.Save();

            // Bước 2: Lấy cái Id vừa sinh ra đó gán cho AccountId của khách hàng
            customer.AccountId = account.Id;

            // Bước 3: Giờ mới lưu Customer vào bảng
            _customerRepo.Add(customer);
            _customerRepo.Save();
        }

        public Customer GetCustomerById(int id)
        {
            return _customerRepo.GetById(id);
        }

        public void UpdateCustomer(Customer customer)
        {
            // Repo đánh dấu thay đổi
            _customerRepo.Update(customer);
            // Lưu xuống DB
            _customerRepo.Save();
        }

        public void DeleteCustomer(int customerId)
        {
            var customer = _customerRepo.GetById(customerId);
            if (customer != null)
            {
                var account = _accountRepo.GetById(customer.AccountId);
                if (account != null)
                {
                    account.IsActive = false; // Xóa mềm
                    _accountRepo.Update(account);
                    _accountRepo.Save();
                }
            }
        }
    }
       
}
