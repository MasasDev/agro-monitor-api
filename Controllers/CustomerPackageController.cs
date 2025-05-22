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
                AssignedAt = DateTime.UtcNow,
            };

            await _db.CustomerPackages.AddAsync(customerPackage);
            await _db.SaveChangesAsync();

             return CreatedAtAction(nameof(AssignPackageToCustomer), new { customerId = customerPackage.CustomerId, packageId = customerPackage.PackageId },
                new AssignedPackageResponseDTO
                {
                    CustomerId = customer.Id,
                    CustomerName = customer.Name,
                    PackageId = package.Id,
                    PackageName = package.Name,
                    AssignedAt = customerPackage.AssignedAt
                });
        }
        [HttpPost]
        public async Task<ActionResult<List<AssignedPackageResponseDTO>>> AssignPackagesToCustomer([FromBody] List<AssignPackageToCustomerDTO> assignPackageToCustomerDTOList)
        {
            if (assignPackageToCustomerDTOList == null || assignPackageToCustomerDTOList.Count == 0)
            {
                return BadRequest("The payload is missing or empty.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customerIds = assignPackageToCustomerDTOList.Select(x => x.CustomerId).Distinct().ToList();
            var packageIds = assignPackageToCustomerDTOList.Select(x => x.PackageId).Distinct().ToList();

            var customers = await _db.Customers.Where(c => customerIds.Contains(c.Id)).ToListAsync();
            var packages = await _db.Packages.Where(p => packageIds.Contains(p.Id)).ToListAsync();

            var assignedPackageResults = new List<AssignedPackageResponseDTO>();
            var newCustomerPackages = new List<CustomerPackage>();

            foreach (var dto in assignPackageToCustomerDTOList)
            {
                var customer = customers.FirstOrDefault(c => c.Id == dto.CustomerId);
                if (customer == null)
                {
                    return NotFound($"Customer with ID {dto.CustomerId} not found.");
                }

                var package = packages.FirstOrDefault(p => p.Id == dto.PackageId);
                if (package == null)
                {
                    return NotFound($"Package with ID {dto.PackageId} not found.");
                }

               
                var alreadyAssigned = await _db.CustomerPackages
                    .AnyAsync(cp => cp.CustomerId == dto.CustomerId && cp.PackageId == dto.PackageId);

                if (alreadyAssigned)
                {
                    return Conflict($"Package '{package.Name}' is already assigned to customer '{customer.Name}'.");
                }

                var assignedTime = DateTime.UtcNow;

                var customerPackage = new CustomerPackage
                {
                    CustomerId = dto.CustomerId,
                    PackageId = dto.PackageId,
                    AssignedAt = assignedTime
                };

                newCustomerPackages.Add(customerPackage);

                assignedPackageResults.Add(new AssignedPackageResponseDTO
                {
                    CustomerId = customer.Id,
                    CustomerName = customer.Name,
                    PackageId = package.Id,
                    PackageName = package.Name,
                    AssignedAt = assignedTime
                });
            }

            await _db.CustomerPackages.AddRangeAsync(newCustomerPackages);
            await _db.SaveChangesAsync();

            return Ok(assignedPackageResults);
        }

    }
}
