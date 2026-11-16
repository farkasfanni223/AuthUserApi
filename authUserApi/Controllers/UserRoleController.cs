using authUserApi.Models;
using authUserApi.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace authUserApi.Controllers
{
    [Route("userrole")]
    [ApiController]
    public class UserRoleController : ControllerBase
    {
        private readonly AuthuserContext _context;

        public UserRoleController(AuthuserContext context)
        {
            _context = context;
        }

        [HttpPost("assignRole")]

        public async Task < IActionResult > AssignRole([FromBody] AssignRoleDto assignRoleDto)
        {
            try
            {
                var user = await _context.Users.Include(u => u.Roles)
                      .SingleOrDefaultAsync(u => u.Id == assignRoleDto.UserId);

                var role = await _context.Roles.FindAsync(assignRoleDto.RoleId);

                if (user is null || role is null)
                {
                    return StatusCode(404, new {message = "Nincs ilyen user vagy role!"});
                }

                var isValid = user.Roles.Any(r => r.Id == assignRoleDto.RoleId);

                if (isValid)
                {
                    return StatusCode(409, new {message = "A User-nek már van ilyen szerepköre!"});
                }

                user.Roles.Add(role);
                await _context.SaveChangesAsync();

                return StatusCode(201, new { message = "Szerepkör felvéve!" });
            }

            catch(Exception e)
            {
                return StatusCode(400, new {message = e.Message});
            }
        }

        [HttpDelete("deleteRole")]

        public async Task<IActionResult> DeleteRole([FromBody] DeleteRoleDto deleteRoleDto)
        {
            try
            {
                var user = await _context.Users.Include(u => u.Roles)
                      .SingleOrDefaultAsync(u => u.Id == deleteRoleDto.UserId);

                var role = await _context.Roles.FindAsync(deleteRoleDto.RoleId);

                if (user is null || role is null)
                {
                    return StatusCode(404, new { message = "Nincs ilyen user vagy role!" });
                }

                //var isValid = user.Roles.Any(r => r.Id == assignRoleDto.RoleId);

                if (!user.Roles.Any(r => r.Id == deleteRoleDto.RoleId))
                {
                    return StatusCode(404, new { message = "A User nem rendelkezik ezzel szerepkörrel!" });
                }

                user.Roles.Remove(role);
                await _context.SaveChangesAsync();

                return StatusCode(200, new { message = "Szerepkör törölve!" });
            }

            catch (Exception e)
            {
                return StatusCode(400, new { message = e.Message });
            }

   }    }
}
