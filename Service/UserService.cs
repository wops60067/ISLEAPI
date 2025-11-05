using Microsoft.IdentityModel.Tokens; 
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;              
using System.Text;   
using ISLE.Interfaces;
using Dapper;
using System.Data;
using Microsoft.Extensions.Configuration; 

namespace ISLE.Services
{
    public class UserService:IUserService
    {
        private readonly IDbConnection _db;
        private readonly IConfiguration _config;

        public UserService(IDbConnection db,IConfiguration config)
        {
            _db = db;
            _config = config;
        }
        //驗證
        public bool EmailExists(string email)
        {
            var sql = "SELECT COUNT(1) FROM Users where email = @email";
            var count = _db.ExecuteScalar<int>(sql, new {email = email});
            return count > 0;
        }
        //註冊
        public bool Register(string userName, string email, string password)
        {
            if(EmailExists(email)) return false;

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var sql = @"INSERT INTO Users (name, email, password_hash)
                        VALUES (@UserName, @Email, @PasswordHash)";

            var rows = _db.Execute(sql, new { UserName = userName, Email = email, PasswordHash = passwordHash });
            return rows > 0;
        }
        //登入
        public object? Login(string email, string password)
        {
            var sql = "select id,name,email,password_hash from Users WHERE email = @Email";
            var user = _db.QueryFirstOrDefault<dynamic>(sql ,new { Email = email });

            if(user == null) return null;

            bool idPasswordValid = BCrypt.Net.BCrypt.Verify(password,(string)user.password_hash);
            if(!idPasswordValid) return null;

            var token = GenerateJwtToken(user.id, user.email, user.name);
            return new{
                ID = user.id,
                Name = user.name,
                Email = user.email,
                Token = token
            };
        }
        private string GenerateJwtToken(int userId, string email, string name)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(JwtRegisteredClaimNames.UniqueName, name)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1), // token 有效時間
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}