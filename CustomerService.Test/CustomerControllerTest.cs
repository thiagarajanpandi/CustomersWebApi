using CustomerService.Controllers;
using CustomerService.Dtos;
using CustomerService.Models;
using CustomerService.Repositories;
using CustomerService.Repositories.DbContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace CustomerService.Test
{
    public class CustomerControllerTest
    {

        [Fact]
        public async Task Can_add_customer()
        {
            var conextOptions = new DbContextOptionsBuilder<CustomerDbContext>()
                                        .UseInMemoryDatabase("CustomerAddTest")
                                        .Options;

            var customerToAdd = new CustomerDto()
            {
                FirstName = "Thiagarajan",
                LastName = "Pandi",
                DateOfBirth = new DateTime(2020, 05, 12)
            };


            using (var context = new CustomerDbContext(conextOptions))
            {
                var controller = new CustomerController(new CustomerRepository(context));
                var customerAdded = ((await controller.AddCustomerAsync(customerToAdd)) as ObjectResult).Value as Customer;

                var customerValidate = ((await controller.GetCustomerAsync(customerAdded.ID)) as ObjectResult).Value as Customer;

                AssertCustomer(customerToAdd, customerValidate);
            }
        }

        [Fact]
        public async Task Can_update_customer()
        {
            var conextOptions = new DbContextOptionsBuilder<CustomerDbContext>()
                            .UseInMemoryDatabase("CustomerUpdateTest")
                            .Options;

            var customerToAdd = new CustomerDto()
            {
                FirstName = "Thiagarajan",
                LastName = "Pandi",
                DateOfBirth = new DateTime(2020, 05, 12)
            };

            int updateId;
            using (var context = new CustomerDbContext(conextOptions))
            {
                var controller = new CustomerController(new CustomerRepository(context));
                var customerAdded = ((await controller.AddCustomerAsync(customerToAdd)) as ObjectResult).Value as Customer;
                updateId = customerAdded.ID;
            }

            var customerToUpdate = new CustomerDto()
            {
                FirstName = "ThiagarajanNew",
                LastName = "PandiNew",
                DateOfBirth = new DateTime(2021, 02, 25)
            };

            using (var context = new CustomerDbContext(conextOptions))
            {
                var controller = new CustomerController(new CustomerRepository(context));
                await controller.UpdateCustomerAsync(updateId, customerToUpdate);

                var customerValidate = ((await controller.GetCustomerAsync(updateId)) as ObjectResult).Value as Customer;

                AssertCustomer(customerToUpdate, customerValidate);
            }
        }

        [Fact]
        public async Task Can_delete_customer()
        {
            var conextOptions = new DbContextOptionsBuilder<CustomerDbContext>()
                            .UseInMemoryDatabase("CustomerDeleteTest")
                            .Options;

            var customerToAdd = new CustomerDto()
            {
                FirstName = "Thiagarajan",
                LastName = "Pandi",
                DateOfBirth = new DateTime(2020, 05, 12)
            };

            int deleteId;
            using (var context = new CustomerDbContext(conextOptions))
            {
                var controller = new CustomerController(new CustomerRepository(context));
                var customerAdded = ((await controller.AddCustomerAsync(customerToAdd)) as ObjectResult).Value as Customer;
                deleteId = customerAdded.ID;
            }

            using (var context = new CustomerDbContext(conextOptions))
            {
                var controller = new CustomerController(new CustomerRepository(context));
                await controller.DeleteCustomerAsync(deleteId);

                var deleteResult = (await controller.GetCustomerAsync(deleteId)) as NotFoundResult;

                Assert.NotNull(deleteResult);
                Assert.Equal(404, deleteResult.StatusCode);//Expect not found

            }
        }

        [Fact]
        public async Task Can_fetch_customers()
        {
            var conextOptions = new DbContextOptionsBuilder<CustomerDbContext>()
                            .UseInMemoryDatabase("CustomerFetchTest")
                            .Options;

            var customer1ToAdd = new CustomerDto()
            {
                FirstName = "Thiagarajan 1",
                LastName = "Pandi 1",
                DateOfBirth = new DateTime(2020, 05, 12)
            };

            var customer2ToAdd = new CustomerDto()
            {
                FirstName = "Thiagarajan 2",
                LastName = "Pandi 2",
                DateOfBirth = new DateTime(2019, 04, 11)
            };


            using (var context = new CustomerDbContext(conextOptions))
            {
                var controller = new CustomerController(new CustomerRepository(context));
                await controller.AddCustomerAsync(customer1ToAdd);
            }

            using (var context = new CustomerDbContext(conextOptions))
            {
                var controller = new CustomerController(new CustomerRepository(context));
                await controller.AddCustomerAsync(customer2ToAdd);
            }

            using (var context = new CustomerDbContext(conextOptions))
            {
                var controller = new CustomerController(new CustomerRepository(context));
                var customers = ((await controller.GetCustomersAsync("an")) as ObjectResult).Value as List<Customer>;

                Assert.Equal(2, customers.Count);

                AssertCustomer(customer1ToAdd, customers[0]);
                AssertCustomer(customer2ToAdd, customers[1]);
            }
        }

        private void AssertCustomer(CustomerDto expectedCustomer, Customer resultCustomer)
        {
            Assert.Equal(expectedCustomer.FirstName, resultCustomer.FirstName);
            Assert.Equal(expectedCustomer.LastName, resultCustomer.LastName);
            Assert.Equal(expectedCustomer.DateOfBirth, resultCustomer.DateOfBirth);
        }
    }
}
