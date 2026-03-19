using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using MotelLeAnh49.Data;

namespace DataAccess.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly MotelDbContext _context;

        public CustomerRepository(MotelDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Customer> GetAll()
        {
            return _context.Customers
                    .Include(c => c.Account)
                    .Where(c => c.Account != null && c.Account.IsActive == true)
                    .ToList();
        }

        public IEnumerable<Customer> SearchByIdentity(string cccd)
        {
            return _context.Customers
                    .Include(c => c.Account)
                    .Where(c => c.IdentityNumber.Contains(cccd)
                                && c.Account != null
                                && c.Account.IsActive == true)
                    .ToList();
        }

        public void Add(Customer customer)
        {
            // Check IdentityNumber
            if (_context.Customers.Any(c => c.IdentityNumber == customer.IdentityNumber))
            {
                throw new Exception("Identity Number exists");
            }

            // Check Phone
            if (_context.Customers.Any(c => c.Phone == customer.Phone))
            {
                throw new Exception("Phone Number exists");
            }

            // Check Email
            if (_context.Customers.Any(c => c.Email == customer.Email && c.Id != customer.Id))
            {
                throw new Exception("Email exists");
            }

            _context.Customers.Add(customer);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public Customer GetById(int id)
        {
            return _context.Customers.FirstOrDefault(c => c.Id == id);
        }

        public void Update(Customer customer)
        {
            // Check IdentityNumber duplicate
            if (_context.Customers.Any(c => c.IdentityNumber == customer.IdentityNumber && c.Id != customer.Id))
            {
                throw new Exception("Identity Number exists");
            }

            // Check Phone duplicate
            if (_context.Customers.Any(c => c.Phone == customer.Phone && c.Id != customer.Id))
            {
                throw new Exception("Phone Number exists");
            }

            _context.Customers.Update(customer);
        }

        public void Delete(Customer customer)
        {
            _context.Customers.Remove(customer);
        }
        public async Task<Customer> GetByIdAsync(int id)
        {
            return await _context.Customers.FindAsync(id);
        }
    }
}