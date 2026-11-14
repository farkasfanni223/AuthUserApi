using authUserApi.Models;
using authUserApi.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace authUserApi.Controllers
{
    [Route("roles")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly AuthuserContext _context;

        public RolesController(AuthuserContext context)
        {
            _context = context;
        }

        [HttpPost("newRole")]
        public async Task<IActionResult> NewRole([FromBody] NewRoleDto newRoleDto)
        {
            try
            {
                var role = new Role
                {
                    Id = Guid.NewGuid(),
                    Name = newRoleDto.Name,
                    RegTime = DateTime.Now
                };

                if (role is null)
                {
                    return StatusCode(404, new { message = "Nem jött létre szerepkör", result = role });
                }

                await _context.Roles.AddAsync(role);
                await _context.SaveChangesAsync();

                return StatusCode(201, new { message = "Felvéve", result = role });
            }

            catch (Exception e)
            {
                return StatusCode(400, new { message = e.Message });
            }


        }
    }
}
