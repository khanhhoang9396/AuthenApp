using Gateway;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthenApp.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel user)
        {
            if (user == null)
            {
                return BadRequest("Invalid client request");
            }

            if (!ValidateUser(user, out UserAccount userAccount))
                return Unauthorized();

            var token = GenerateToken(userAccount);
            var refreshToken = GenerateRefreshToken(userAccount);
            return Ok( new { AccessToken = token, RefreshToken = refreshToken });
        }

        [HttpPost("RefreshToken")]
        public IActionResult RefreshToken([FromBody] LoginByRefreshToken loginByRefreshToken)
        {
            if (string.IsNullOrWhiteSpace(loginByRefreshToken.RefreshToken) || string.IsNullOrWhiteSpace(loginByRefreshToken.AccessToken))
            {
                return BadRequest("Invalid token");
            }

            //optional
            // Validate expired token
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = "https://localhost:44389/",
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SecureKey1234567")),
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(loginByRefreshToken.AccessToken, validationParameters, out var securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null) { return Unauthorized(); }

            //Get user by refresh token
            var userAccount = GetUserAccountByRefreshToken(loginByRefreshToken.RefreshToken);
            if (userAccount == null)
            {
                return Unauthorized();
            }

            //Generate new token and refresh token
            var token = GenerateToken(userAccount);
            var newRefreshToken = GenerateRefreshToken(userAccount);

            return Ok(new { AccessToken = token, RefreshToken = newRefreshToken });
        }

        private List<Claim> GetUserClaims(UserAccount user)
        {
            var claims = new List<Claim>
            {
                new(nameof(user.UserName),user.UserName ),
                new(nameof(user.Email),user.Email ),
                new(nameof(user.Gender),user.Gender ),
                new(ClaimTypes.DateOfBirth, user.BirthDay),
                new(nameof(user.Country),user.Country ),
                new("Role",user.Role )
            };
            return claims;
        }

        private bool ValidateUser(LoginModel user, out UserAccount userAccount)
        {
            userAccount = DataHelper.UserAccounts.Find(x => x.UserName == user.UserName && x.Password == user.Password);
            return userAccount != null;
        }

        private string GenerateToken(UserAccount userAccount)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var claims = GetUserClaims(userAccount);

            var key = Encoding.UTF8.GetBytes("SecureKey1234567");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(30),
                Issuer = "https://localhost:44389/",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(securityToken);
        }

        private string GenerateRefreshToken(UserAccount userAccount)
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            var refreshToken = Convert.ToBase64String(randomNumber);

            DataHelper.RefreshTokens.Add(new RefreshTokenModel()
            {
                UserId = userAccount.UserId,
                RefreshToken = refreshToken,
                Expires = DateTime.UtcNow.AddDays(7)
            });
            return refreshToken;
        }

        private UserAccount GetUserAccountByRefreshToken(string refreshToken)
        {
           var token =  DataHelper.RefreshTokens.Find(x => x.RefreshToken == refreshToken);
           if (token == null || token.Expires < DateTime.UtcNow)
           {
               return null;
           }

           var user = DataHelper.UserAccounts.Find(x => x.UserId == token.UserId);
           return user;
        }

        [HttpPost("AddUserToBlackList")]
        public IActionResult AddUserToBlackList([FromBody] LoginModel user)
        {
            if (user == null)
            {
                return BadRequest("Invalid client request");
            }
            BlackListHelper.AddBlackList(user.UserName);
            
            return Ok();
        }
    }
}
