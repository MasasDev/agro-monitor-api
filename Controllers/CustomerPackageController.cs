using AgroMonitor.Data;
using AgroMonitor.DTOs;
using AgroMonitor.Migrations;
using AgroMonitor.Models;
using Microsoft.AspNetCore.Mvc;

namespace AgroMonitor.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerPackageController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public CustomerPackageController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<ActionResult> AssignPackageToCustomer([FromBody]AssignPackageToCustomerDTO assignPackageToCustomer)
        {
            if(assignPackageToCustomer == null)
            {
                return BadRequest("The payload is missing");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var package = await _db.Packages.FindAsync(assignPackageToCustomer.PackageId);

            if (package == null)
            {
                return NotFound("Package not found");
            }

            var customer = await _db.Customers.FindAsync(assignPackageToCustomer.CustomerId);

            if(customer == null)
            {
                return NotFound("Customer not found");
            }

            CustomerPackage customerPackage = new CustomerPackage
            {
                PackageId = assignPackageToCustomer.PackageId,
                CustomerId = assignPackageToCustomer.CustomerId,
            };

            await _db.CustomerPackages.AddAsync(customerPackage);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(AssignPackageToCustomer), new { customerId = customerPackage.CustomerId, packageId = customerPackage.PackageId }, customerPackage);
        }
    }
}
