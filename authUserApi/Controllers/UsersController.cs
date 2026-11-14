using authUserApi.Models;
using authUserApi.Models.DTOs;
using authUserApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace authUserApi.Controllers
{
    [Route("users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AuthuserContext _context;
        private readonly PasswordHasher _passwordHasher;
        public UsersController(AuthuserContext context, PasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        [HttpPost("newUser")]
        public async Task<IActionResult> NewUser([FromBody] NewUserDto newUserDto)
        {
            try
            {
                (var hash, var salt) = _passwordHasher.HashPassword(newUserDto.Password);

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Name = newUserDto.Name,
                    Email = newUserDto.Email,
                    Password = hash,
                    Salt = salt,
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
