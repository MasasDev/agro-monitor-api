using AgroMonitor.Data;
using AgroMonitor.DTOs;
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
            var customer = await _db.Customers.Include(c => c.CustomerPackages).ThenInclude(cp => cp.Package).FirstOrDefaultAsync(c => c.Id == id);

            if(customer == null)
            {
                return NotFound("Customer not found");
            }

            CustomerDTO customerDTO = ToCustomerDTO(customer);

            return Ok(customerDTO);
        }

        [HttpGet]
        public async Task<ActionResult<List<CustomerDTO>>> GetCustomers()
        {
            List<CustomerDTO> customers = await _db.Customers.Include(c => c.CustomerPackages).ThenInclude(cp => cp.Package).Select(c => ToCustomerDTO(c)).ToListAsync();

            return Ok(customers);
        }

        [HttpPost]
        public async Task<ActionResult<CustomerDTO>> AddCustomer([FromBody] AddCustomerDTO addCustomerDTO)
        {
            if (addCustomerDTO == null)
            {
                return BadRequest("Add payload is missing");
            }

            if (string.IsNullOrEmpty(addCustomerDTO.Name) || string.IsNullOrEmpty(addCustomerDTO.PhoneNumber))
            {
                return BadRequest("The customer is missing important details");
            }

            string customerUniqueIdentifier = Guid.NewGuid().ToString("N")[..10].ToUpper();

            while(await _db.Customers.AnyAsync(c => c.CustomerUniqueIdentifier == customerUniqueIdentifier))
            {
                customerUniqueIdentifier = Guid.NewGuid().ToString("N")[..10].ToUpper();
            }

            Customer newCustomer = new Customer
            {
                CustomerUniqueIdentifier = customerUniqueIdentifier,
                Name = addCustomerDTO.Name,
                PhoneNumber = addCustomerDTO.PhoneNumber,
                Email = addCustomerDTO.Email,
                FarmLocation = addCustomerDTO.FarmLocation,
                RegistrationDate = DateTime.UtcNow,
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

            return CreatedAtAction(nameof(GetCustomer), new {newCustomer.Id}, ToCustomerDTO(newCustomer));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCustomer(long id, UpdateCustomerDTO updateCustomerDTO)
        {
            if (updateCustomerDTO == null)
            {
                return BadRequest("Update payload is missing.");
            }

            if (id != updateCustomerDTO.Id)
            {
                return BadRequest("The ID in the route does not match the ID in the payload.");
            }

            var customer = await _db.Customers.FirstOrDefaultAsync(c => c.Id == id);

            if(customer == null)
            {
                return BadRequest("Customer not found");
            }

            customer.Name = updateCustomerDTO.Name;
            customer.PhoneNumber = updateCustomerDTO.PhoneNumber;
            customer.Email = updateCustomerDTO.Email;
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
                return NotFound("Customer not found");
            }

             _db.Customers.Remove(customer);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        private static CustomerDTO ToCustomerDTO (Customer customer)
        {
            return new CustomerDTO
            {
                Id = customer.Id,
                CustomerUniqueIdentifier = customer.CustomerUniqueIdentifier,
                Name = customer.Name,
                PhoneNumber = customer.PhoneNumber,
                Email = customer.Email,
                RentedDevicesReturnDate = customer.RentedDevicesReturnDate,
                FarmLocation = customer.FarmLocation,
                AreRentedDevicesReturned = customer.AreRentedDevicesReturned,
                RegistrationDate = customer.RegistrationDate,
                Packages = customer.CustomerPackages.Select(cp => new PackageDTO
                {
                    Id = cp.Package.Id,
                    Name = cp.Package.Name,
                    Price = cp.Package.Price,
                    Description = cp.Package.Description,
                    CreatedAt = cp.Package.CreatedAt,
                    LastUpdatedAt = cp.Package.LastUpdatedAt,
                    DeviceId = cp.Package.DeviceId,
                }).ToList(),
            };
        }

    }
}
