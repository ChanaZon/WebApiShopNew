using Repositories;
using Entities.Models;
using DTO;
using Zxcvbn;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> Login(string password, string userName)
        {
            var user = await _userRepository.Login(userName);
            if (user == null || string.IsNullOrEmpty(user.Salt))
                return null;

            string hashedInputPassword = HashPassword(password, user.Salt);
            if (hashedInputPassword == user.Password)
                return user;

            return null;
        }

        public async Task<User> AddUser(User user)
        {
            int passwordScore = CheckPassword(user.Password);
            if (string.IsNullOrEmpty(user.UserName) ||
                string.IsNullOrEmpty(user.FirstName) ||
                string.IsNullOrEmpty(user.LastName) ||
                passwordScore < 2)
            {
                return null;
            }

            string salt = GenerateSalt();
            string hashedPassword = HashPassword(user.Password, salt);

            user.Salt = salt;
            user.Password = hashedPassword;

            return await _userRepository.AddUserAsync(user);
        }

        public async Task<User> UpdateUser(int id, User userToUpdate)
        {
            int passwordScore = CheckPassword(userToUpdate.Password);
            if (string.IsNullOrEmpty(userToUpdate.UserName) ||
                string.IsNullOrEmpty(userToUpdate.FirstName) ||
                string.IsNullOrEmpty(userToUpdate.LastName) ||
                passwordScore < 2)
            {
                return null;
            }

            string salt = GenerateSalt();
            string hashedPassword = HashPassword(userToUpdate.Password, salt);

            userToUpdate.Salt = salt;
            userToUpdate.Password = hashedPassword;

            return await _userRepository.UpdateUserAsync(id, userToUpdate);
        }

        public int CheckPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                return -1;

            var result = Zxcvbn.Core.EvaluatePassword(password);
            return result.Score;
        }

        private string GenerateSalt(int size = 8)
        {
            var rng = new RNGCryptoServiceProvider();
            byte[] saltBytes = new byte[size];
            rng.GetBytes(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }

        private string HashPassword(string password, string salt)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] combinedBytes = Encoding.UTF8.GetBytes(password + salt);
                byte[] hashBytes = sha256.ComputeHash(combinedBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}
