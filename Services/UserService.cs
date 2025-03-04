using Repositories;
using Entities.Models;
using System.Text.Json;
using Zxcvbn;
using DTO;
using System.ComponentModel.DataAnnotations;

namespace Services
{
    public class UserService : IUserService
    {
        IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }



        public async Task<User> Login(string Password, string UserName)
        {
            return await _userRepository.Login(Password,UserName);
        }

        public async Task<User> UpdateUser(int id, User userToUpdate)
        {
            int passwordScore = CheckPassword(userToUpdate.Password);
            if (userToUpdate.UserName == "" || userToUpdate.FirstName == "" || userToUpdate.LastName == "" || passwordScore < 2)
            {
                return null;
            }
            return await _userRepository.UpdateUserAsync(id, userToUpdate);
        }

        public int CheckPassword(string password)
        {
            if (password == null || password == "")
            {
                return -1;
            }
            var result = Zxcvbn.Core.EvaluatePassword(password);

            return result.Score;
        }

  
        public async Task<User> AddUser(User user)
        {
            int passwordScore = CheckPassword(user.Password);
            if (user.UserName == "" || user.FirstName == "" || user.LastName == "" || passwordScore < 2)
            {
                return null;
            }
            return await _userRepository.AddUserAsync(user);
        }

    }
}

