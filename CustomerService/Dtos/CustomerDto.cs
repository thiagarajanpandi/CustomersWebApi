using System;
using CustomerService.Models;

namespace CustomerService.Dtos
{
    public class CustomerDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }

        public Customer ToEntity(int? customerId = null)
        {
            return new Customer()
            {
                ID = customerId ?? 0,
                FirstName = this.FirstName,
                LastName = this.LastName,
                DateOfBirth = this.DateOfBirth,
            };
        }

    }
}
