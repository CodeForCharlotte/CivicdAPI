using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using CivicdAPI.Web.Entities;
using CivicdAPI.Web.Models;
using CivicdAPI.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace CivicdAPI.Web.Controllers
{
    [Authorize]
    [Route("api/Account")]
    public class AccountController : ControllerBase
    {
        private ApplicationDbContext _context;
        private IConfiguration _configuration;

        public AccountController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("token")]
        public IActionResult RequestToken([FromBody]TokenModel request)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserName == request.Username);
            if (user == null)
            {
                throw new Exception($"Unable to find a user with Username: {request.Username}");
            }


            if (!VerifyPassword(user.PasswordHash, request.Password))
            {
                return BadRequest("Could not verify username and password");
            }

            var token = new
            {
                Token = GenerateToken(request),
                UserId = user.Id
            };

            return Ok(token);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        public IActionResult Register([FromBody]RegisterUserModel request)
        {
            var hash = GenerateHash(request.Password);

            var existingUser = _context.Users.FirstOrDefault(u => u.UserName == request.UserName);

            if (existingUser != null)
            {
                return BadRequest("this username is taken");
            }

            var user = new ApplicationUser()
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PasswordHash = hash,
                UserName = request.UserName
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(user);
        }

        #region Helpers

        public string GenerateHash(string plainTextPassword)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: plainTextPassword,
                salt: new byte[0],
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
        }

        public bool VerifyPassword(string passwordHash, string plainTextPassword)
        {
            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            string hashed = GenerateHash(plainTextPassword);

            return hashed == passwordHash;
        }

        public string GenerateToken(TokenModel request)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, request.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecurityKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Uri"],
                audience: _configuration["Uri"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(90),
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        #endregion
    }
}
