using authUserApi.Models;
using authUserApi.Models.DTOs;
using authUserApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        [HttpPost("login")]
        public async Task <IActionResult> Login([FromBody] LoginUserDto loginUserDto)
        {
            try
            {
                var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == loginUserDto.Email);

                if (user is null)
                {
                    return StatusCode(404, new { message = "Nincs ilyen email!" });
                }

                var isValid = _passwordHasher.Verify(loginUserDto.Password!, user.Password!, user.Salt!);

                if (isValid == true)
                {
                    return StatusCode(200, new { message = "Sikeres bejelentkezés!", result = user });
                }

                return StatusCode(401, new { message = "Hibás email vagy jelszó!", result = user });
            }

            catch (Exception e)
            {
                return StatusCode(400, new { message = e.Message });
            }
        }
        
    }
}
