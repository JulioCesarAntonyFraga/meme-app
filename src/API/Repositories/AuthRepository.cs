
using API.Interfaces;
using API.Models;
using API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using API.Configurations;

namespace API.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private MongoClient _client;
        private IMongoDatabase _database;
        //private FunnyAppDatabaseSettings _settings;
        private string collection = "Users";

        public AuthRepository()
        {
            //_settings = new FunnyAppDatabaseSettings();
            //_client = new MongoClient(_settings.ConnectionString);
            //_database = _client.GetDatabase(_settings.DatabaseName);
            //collection = _settings.UsersCollectionName;
            _client = new MongoClient("mongodb://localhost:27017");
            _database = _client.GetDatabase("FunnyApp");
        }

        public async Task<LoginSuccess> Login(Login login)
        {
            var _collection = _database.GetCollection<User>(collection);
            User user = await _collection.Find(x => x.Email == login.Email).FirstOrDefaultAsync();

            if (user == null)
                return null;

            bool verified = BCrypt.Net.BCrypt.Verify(login.Password, user.Password);

            if (!verified)
                return null;

            var refreshToken = API.Services.TokenService.GenerateRefreshToken();
            user.RefreshToken = refreshToken;

            await _collection.ReplaceOneAsync(x => x.Id == user.Id, user);

            user.Password = "";

            return new LoginSuccess
            {
                User = user,
                Token = API.Services.TokenService.GenerateToken(user),
                RefreshToken = user.RefreshToken
            };
        }

        public async Task<LoginSuccess> RegisterUser(Register register)
        {
            var _collection = _database.GetCollection<User>(collection);
            var users = await _collection.Find(x => x.Email == register.Email).ToListAsync();
            bool userExists = users.Any();

            if (userExists)
                return null;

            var refreshToken = API.Services.TokenService.GenerateRefreshToken();

            User user = new User
            {
                UserName = register.UserName,
                Email = register.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(register.Password),
                Role = "User",
                RefreshToken = refreshToken
            };

            await _collection.InsertOneAsync(user);

            user.Password = "";

            return new LoginSuccess{
                User = user,
                Token = API.Services.TokenService.GenerateToken(user),
                RefreshToken=refreshToken
            };
        }

        //TODO: Create admin registering method

        public async Task<RefreshTokenSuccess> RefreshToken(RefreshTokenRequest refreshTokenRequest)
        {
            var _collection = _database.GetCollection<User>(collection);
            User user = await _collection.Find(x => x.Id == refreshTokenRequest.UserId).FirstOrDefaultAsync();

            string savedRefreshToken = user.RefreshToken;

            if (savedRefreshToken != refreshTokenRequest.RefreshToken)
                throw new SecurityTokenException("Invalid Token");

            var newJwtToken = API.Services.TokenService.GenerateToken(user);

            var newRefreshToken = API.Services.TokenService.GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;

            await _collection.ReplaceOneAsync(x => x.Id == user.Id, user);

            return new RefreshTokenSuccess
            {
                Token = newJwtToken,
                RefreshToken = newRefreshToken
            };
        }
    }
}
