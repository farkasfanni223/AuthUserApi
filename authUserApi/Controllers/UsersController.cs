using authUserApi.Models;
using authUserApi.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace authUserApi.Controllers
{
    [Route("users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AuthuserContext _context;

        public UsersController(AuthuserContext context)
        {
            _context = context;
        }

        [HttpPost("newUser")]
        public async Task<IActionResult> NewUser([FromBody] NewUserDto newUserDto)
        {
            try
            {
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Name = newUserDto.Name,
                    Email = newUserDto.Email,
                    Password = newUserDto.Password,
                    RegTime= DateTime.Now
                };

                if (user is null)
                {
                    return StatusCode(404, new { message = "Nem jött létre user", result = user });
                }

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                return StatusCode(201, new { message = "Felvéve", result = user });
            }

            catch (Exception e)
            {
                return StatusCode(400, new { message = e.Message });
            }

            
        }
        
    }
}
