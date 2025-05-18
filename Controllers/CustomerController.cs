using System.Net.NetworkInformation;
using System.Reflection.Metadata.Ecma335;
using AgroMonitor.Data;
using AgroMonitor.DTOs;
using AgroMonitor.Migrations;
using AgroMonitor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgroMonitor.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public CustomerController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDTO>> GetCustomer(int id)
        {
            var customer = await _db.Customers.FirstOrDefaultAsync(c => c.Id == id);

            if(customer == null)
            {
                return NotFound("This customer does not exist in the system");
            }

            CustomerDTO customerDTO = ToCustomerDTO(customer);

            return Ok(customerDTO);
        }

        [HttpGet]
        public async Task<ActionResult<List<CustomerDTO>>> GetCustomers()
        {
            List<CustomerDTO> customers = await _db.Customers.Select(c => ToCustomerDTO(c)).ToListAsync();

            return Ok(customers);
        }
        [HttpPost]
        public async Task<ActionResult<AddCustomerDTO>> AddCustomer([FromBody] AddCustomerDTO addCustomerDTO)
        {
            if (addCustomerDTO == null)
            {
                return BadRequest("There is no customer to add");
            }

            if (string.IsNullOrEmpty(addCustomerDTO.Name) || string.IsNullOrEmpty(addCustomerDTO.PhoneNumber))
            {
                return BadRequest("The customer is missing important details");
            }

            string uniqueIdentifier = Guid.NewGuid().ToString("N")[..10].ToUpper();

            while(await _db.Customers.AnyAsync(c => c.CustomerUniqueIdentifier == uniqueIdentifier))
            {
                uniqueIdentifier = Guid.NewGuid().ToString("N")[..10].ToUpper();
            }

            Customer newCustomer = new Customer
            {
                Name = addCustomerDTO.Name,
                PhoneNumber = addCustomerDTO.PhoneNumber,
                CustomerUniqueIdentifier = uniqueIdentifier,
                Email = addCustomerDTO.Email,
                Equipment = addCustomerDTO.Equipment,
                EquipmentReturnDate = null,
                RegistrationDate = DateTime.UtcNow,
                FarmLocation = addCustomerDTO.FarmLocation,
                IsEquipmentReturned = false,
            };

            await _db.Customers.AddAsync(newCustomer);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("IX_Customers_CustomerUniqueIdentifier") == true)
            {
                return StatusCode(500, "A unique identifier collision occurred. Please try again.");
            }

            return CreatedAtAction(nameof(GetCustomer), new {newCustomer.Id, newCustomer.CustomerUniqueIdentifier}, ToCustomerDTO(newCustomer));

        }
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCustomer(long id, UpdateCustomerDTO updateCustomerDTO)
        {
            if(id != updateCustomerDTO.Id)
            {
                return BadRequest("The ID in the route does not match the ID in the payload.");
            }

            if (updateCustomerDTO == null)
            {
                return BadRequest("Update payload is missing.");
            }

            var customer = await _db.Customers.FirstOrDefaultAsync(c => c.Id == id);

            if(customer == null)
            {
                return BadRequest("This customer does not exist in the system");
            }

            if(updateCustomerDTO.IsEquipmentReturned)
            {
                customer.EquipmentReturnDate = DateTime.UtcNow;
            }

            customer.Name = updateCustomerDTO.Name;
            customer.PhoneNumber = updateCustomerDTO.PhoneNumber;
            customer.Equipment = updateCustomerDTO.Equipment;
            customer.Email = updateCustomerDTO.Email;
            customer.IsEquipmentReturned = updateCustomerDTO.IsEquipmentReturned;
            customer.FarmLocation = updateCustomerDTO.FarmLocation;

            _db.Customers.Update(customer);
            await _db.SaveChangesAsync();
        
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> RemoveCustomer(int id)
        {
            var customer = await _db.Customers.FirstOrDefaultAsync(c => c.Id == id);

            if(customer == null)
            {
                return NotFound("This customer does not exist in the system");
            }

             _db.Customers.Remove(customer);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        private static CustomerDTO ToCustomerDTO (Customer customer)
        {
            return new CustomerDTO
            {
                CustomerUniqueIdentifier = customer.CustomerUniqueIdentifier,
                Name = customer.Name,
                PhoneNumber = customer.PhoneNumber,
                Email = customer.Email,
                Equipment = customer.Equipment,
                EquipmentReturnDate = customer.EquipmentReturnDate,
                FarmLocation = customer.FarmLocation,
                IsEquipmentReturned = customer.IsEquipmentReturned,
                RegistrationDate = customer.RegistrationDate,
            };
        }

    }
}
