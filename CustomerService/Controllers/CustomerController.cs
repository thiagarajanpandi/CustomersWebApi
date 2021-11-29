using CustomerService.Dtos;
using CustomerService.Models;
using CustomerService.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;
        public CustomerController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(Customer))]
        public async Task<IActionResult> AddCustomerAsync([FromBody] CustomerDto customerDto)
        {
            if (!ValidateCustomerDto(customerDto))
            {
                return BadRequest();
            }

            var customer = await _customerRepository.AddCustomerAsync(customerDto.ToEntity());

            return Ok(customer);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomerAsync(int id, [FromBody] CustomerDto customerDto)
        {
            if (!ValidateCustomerDto(customerDto))
            {
                return BadRequest();
            }

            if (!await _customerRepository.UpdateCustomerAsync(customerDto.ToEntity(id)))
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomerAsync(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            if (!await _customerRepository.DeleteCustomerAsync(id))
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Customer))]
        public async Task<IActionResult> GetCustomerAsync(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var customer = await _customerRepository.GetCustomerAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer);
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<Customer>))]
        public async Task<IActionResult> GetCustomersAsync(string filter)
        {
            if (filter == null) return BadRequest("Filter empty");

            var customers = await _customerRepository.GetCustomersAsync(filter);
            return Ok(customers);
        }

        private bool ValidateCustomerDto(CustomerDto customerDto)
        {
            if (string.IsNullOrWhiteSpace(customerDto.FirstName) || string.IsNullOrWhiteSpace(customerDto.LastName) ||
                customerDto.DateOfBirth == DateTime.MinValue)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

    }
}
