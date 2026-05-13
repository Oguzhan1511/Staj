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


using kitap.Dtos;

namespace kitap.Controllers;


[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;
    public UsersController(AppDbContext context) => _context = context;

    [HttpGet]
public async Task<ActionResult<IEnumerable<User>>> GetUsers([FromQuery] UsersFilterDto filter)
{
    var query = _context.Users.AsQueryable();

    
    if (!string.IsNullOrEmpty(filter.SearchTerm))
    {
        query = query.Where(u => 
            u.Name.Contains(filter.SearchTerm) || 
            u.LastName.Contains(filter.SearchTerm));
    }

    return await query.ToListAsync();
}

    [HttpPost]
    public async Task<ActionResult<User>> PostUser(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return Ok(user);
    }
   

    


    }
