
using CustomerService.Models;
using CustomerService.Repositories.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerService.Repositories
{
    public interface ICustomerRepository
    {
        public Task<Customer> AddCustomerAsync(Customer customer);
        public Task<bool> UpdateCustomerAsync(Customer customer);
        public Task<bool> DeleteCustomerAsync(int customerID);
        public Task<Customer> GetCustomerAsync(int customerID);
        public Task<List<Customer>> GetCustomersAsync(string filter);
    }

    public class CustomerRepository : ICustomerRepository
    {
        private readonly CustomerDbContext _customerDbContext;

        public CustomerRepository(CustomerDbContext customerDbContext)
        {
            _customerDbContext = customerDbContext;
        }

        public async Task<Customer> AddCustomerAsync(Customer customer)
        {
            _customerDbContext.Customers.Add(customer);
            await _customerDbContext.SaveChangesAsync();
            return customer;
        }

        public async Task<bool> UpdateCustomerAsync(Customer customer)
        {
            try
            {
                _customerDbContext.Customers.Update(customer);
                await _customerDbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                //Log exception
                return false;
            }

            return true;
        }

        public async Task<bool> DeleteCustomerAsync(int customerID)
        {
            var customerToDelete = await _customerDbContext.Customers.SingleOrDefaultAsync(c => c.ID == customerID);
            if (customerToDelete == null)
            {
                return false;
            }

            try
            {
                _customerDbContext.Customers.Remove(customerToDelete);
                await _customerDbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                //Log exception
                return false;
            }

            return true;
        }

        public async Task<Customer> GetCustomerAsync(int customerID)
        {
            return await _customerDbContext.Customers.SingleOrDefaultAsync(c => c.ID == customerID);
        }

        public async Task<List<Customer>> GetCustomersAsync(string filter)
        {
            return await _customerDbContext.Customers
                                            .Where(c => c.FirstName.Contains(filter, System.StringComparison.OrdinalIgnoreCase)
                                                                        || c.LastName.Contains(filter, System.StringComparison.OrdinalIgnoreCase))
                                            .ToListAsync();
        }
    }
}
