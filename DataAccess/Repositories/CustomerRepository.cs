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
                   .ToList();
        }

        public void Add(Customer customer) => _context.Customers.Add(customer);

        public void Save() => _context.SaveChanges();

        public Customer GetById(int id)
        {
            return _context.Customers.FirstOrDefault(c => c.Id == id);
        }

        public void Update(Customer customer)
        {
            // Cách này sẽ cập nhật toàn bộ các trường của Customer
            _context.Customers.Update(customer);
        }
    }
}
