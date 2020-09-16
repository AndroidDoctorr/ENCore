using ElevenNote.Data;
using ElevenNote.Data.Entities;
using ElevenNote.Models.Token;
using ElevenNote.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ElevenNote.Services.User
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        public UserService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<bool> RegisterUser(UserRegister model)
        {
            if (await GetUserByEmail(model.Email) != null || await GetUserByUsername(model.Username) != null)
                return false;

            var entity = new UserEntity
            {
                Email = model.Email,
                Username = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName,
                DateCreated = DateTime.UtcNow
            };

            var passwordHasher = new PasswordHasher<UserEntity>();
            entity.Password = passwordHasher.HashPassword(entity, model.Password);

            _context.Users.Add(entity);
            return await _context.SaveChangesAsync() == 1;
        }

        public async Task<UserDetail> GetUserById(int userId)
        {
            var entity = await _context.Users.FindAsync(userId);
            if (entity == null)
                return null;

            var detail = new UserDetail
            {
                Id = entity.Id,
                Email = entity.Email,
                Username = entity.Username,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                DateCreated = entity.DateCreated
            };

            return detail;
        }

        public async Task<TokenResponse> GetToken(TokenRequest model)
        {
            var userEntity = await GetUserByUsername(model.Username);
            if (model == null)
                return null;

            // using System.IdentityModel.Tokens.Jwt;
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim("Id", userEntity.Id.ToString()),
                new Claim("FirstName", userEntity.FirstName),
                new Claim("LastName", userEntity.LastName),
                new Claim("Username", userEntity.Username),
                new Claim("Email", userEntity.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(issuer: _configuration["Jwt:Issuer"],
                                             audience: _configuration["Jwt:Audience"],
                                             claims: claims,
                                             notBefore: DateTime.UtcNow,
                                             expires: DateTime.UtcNow.AddDays(15),
                                             signingCredentials: signIn);

            var tokenResponse = new TokenResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                IssuedAt = token.ValidFrom,
                Expires = token.ValidTo
            };
            return tokenResponse;
        }

        private async Task<UserEntity> GetUserByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        private async Task<UserEntity> GetUserByUsername(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
        }
    }
}
