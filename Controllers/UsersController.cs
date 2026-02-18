using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kitap.Data;
using kitap.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using BCrypt.Net;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace kitap.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;
    public UsersController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers() => await _context.Users.ToListAsync();

    [HttpPost]
    public async Task<ActionResult<User>> PostUser(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return Ok(user);
    }
    [HttpPost("register")]
    public async Task<ActionResult<User>> Register(User user)
    {
        //Alan boş mu kontrolü
        if(!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        //mail önceden kullanılmış mı kontrolü
        var userExists = await _context.Users.AnyAsync(u => u.Mail == user.Mail);
        if (userExists)
        {
            return BadRequest("Bu e-posta adresi zaten kullanımda.");
        }
        //şifreyi hashlıyoruz
        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        //şifreyi gizliyoruz
        user.Password = null;
        return Ok(new { message = "Kayıt başarıyla tamamlandı.", user });

    }

    [HttpPost("login")]

    public async Task<IActionResult> Login([FromBody] User loginUser)
    {
        //Kullanıcıyı eşleştir
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Mail == loginUser.Mail);

        //kullanıcı varsa şifresi doğru mu
        if (user == null || !BCrypt.Net.BCrypt.Verify(loginUser.Password, user.Password))
    {
        return Unauthorized("E-posta veya şifre hatalı");
    }

    //token oluşturma
    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.ASCII.GetBytes("BuGizliBirSifredir123456!");
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user.Id.ToString()) }),
        Expires = DateTime.UtcNow.AddDays(1), 
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };
    var token = tokenHandler.CreateToken(tokenDescriptor);
    var tokenString = tokenHandler.WriteToken(token);

    return Ok(new { Token = tokenString, User = user.Name });
}


    }
