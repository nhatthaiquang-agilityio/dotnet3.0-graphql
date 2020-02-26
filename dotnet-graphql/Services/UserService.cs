using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using dotnet_graphql.Data;
using dotnet_graphql.Helpers;
using dotnet_graphql.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace dotnet_graphql.Services
{
    public interface IUserService
    {
        User Authenticate(string username, string password);

        User CreateUser(UserViewModel user);

        IEnumerable<User> GetAll();
    }

    public class UserService : IUserService
    {
        // users hardcoded for simplicity, store in a db with hashed passwords in production applications
        private readonly List<User> _users = new List<User>
        {
            new User { Id = 1, FirstName = "Test", LastName = "User", Username = "test", Password = "Abcde@123" }
        };

        private readonly AppSettings _appSettings;
        private readonly AppDbContext _appDbContext;

        public UserService(AppDbContext appDbContext, IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
            _appDbContext = appDbContext;
        }

        public User Authenticate(string username, string password)
        {
            var user = _users.SingleOrDefault(x => x.Username == username && x.Password == password);

            // return null if user not found
            if (user == null)
                return null;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            return user.WithoutPassword();
        }

        public IEnumerable<User> GetAll()
        {
            return _users.WithoutPasswords();
        }

        public User CreateUser(UserViewModel userViewModel)
        {
            var user = _users.SingleOrDefault(x => x.Username == userViewModel.Username);

            // user exists
            if (user != null)
                return null;

            var userModel = new User
            {
                FirstName = userViewModel.FirstName,
                LastName = userViewModel.LastName,
                Username = userViewModel.Username,
                Password = userViewModel.Password
            };
            _appDbContext.Users.Add(userModel);
            _appDbContext.SaveChangesAsync();
            return userModel;
        }
    }
}
