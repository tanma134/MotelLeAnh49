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
        private readonly MotelDbContext _context;

        public CustomerService(ICustomerRepository customerRepo, IAccountRepository accountRepo, MotelDbContext context)
        {
            _customerRepo = customerRepo;
            _accountRepo = accountRepo;
            _context = context;
        }

        public IEnumerable<Customer> GetAllCustomers()
        {
            return _customerRepo.GetAll();
        }
        public bool IsEmailDuplicate(string email, int customerId)
        {
            return _context.Customers.Any(c =>
                c.Email == email &&
                c.Id != customerId);
        }
        public bool IsPhoneDuplicate(string phone, int customerId)
        {
            return _context.Customers.Any(c =>
                c.Phone == phone &&
                c.Id != customerId);
        }
        public void CreateCustomer(Customer customer, Account account)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _accountRepo.Add(account);
                    _accountRepo.Save();

                    customer.AccountId = account.Id;
                    customer.Account = null;

                    _customerRepo.Add(customer);
                    _customerRepo.Save();

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public Customer GetCustomerById(int id)
        {
            return _customerRepo.GetById(id);
        }


       public void UpdateCustomer(Customer customer)
    {
        // 🆕 Kiểm tra email trùng lặp
        if (IsEmailDuplicate(customer.Email, customer.Id))
        {
            throw new Exception("Email này đã được sử dụng bởi khách hàng khác!");
        }

        // 🆕 Kiểm tra phone trùng lặp
        if (IsPhoneDuplicate(customer.Phone, customer.Id))
        {
            throw new Exception("Số điện thoại này đã được sử dụng bởi khách hàng khác!");
        }

        using (var transaction = _context.Database.BeginTransaction())
        {
            try
            {
                _customerRepo.Update(customer);
                _customerRepo.Save();

                var account = _accountRepo.GetById(customer.AccountId);
                if (account != null && account.Email != customer.Email)
                {
                    account.Email = customer.Email;
                    _accountRepo.Update(account);
                    _accountRepo.Save();
                }

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
            }
        }

        public void DeleteCustomer(int customerId)
        {
            // Check khách có booking đang hoạt động không
            var hasActiveBooking = _context.Bookings.Any(b =>
                b.CustomerId == customerId &&
                (b.Status == "Pending" || b.Status == "Confirmed"));

            if (hasActiveBooking)
            {
                throw new Exception("Khách hàng đang có đơn đặt phòng chưa hoàn thành. Không thể xóa!");
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var customer = _customerRepo.GetById(customerId);
                    if (customer != null)
                    {
                        var account = _accountRepo.GetById(customer.AccountId);
                        if (account != null)
                        {
                            account.IsActive = false; // soft delete account
                            _accountRepo.Update(account);
                            _accountRepo.Save();
                        }

                        _customerRepo.Delete(customer);
                        _customerRepo.Save();

                        transaction.Commit();
                    }
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new Exception("Có lỗi xảy ra khi xóa dữ liệu hệ thống.");
                }
            }
        }

        public IEnumerable<Customer> SearchByIdentity(string cccd)
        {
            return _customerRepo.SearchByIdentity(cccd);
        }
        public Customer GetProfile(int accountId)
        {
            return _context.Customers
                .FirstOrDefault(c => c.AccountId == accountId);
        }

    }
}