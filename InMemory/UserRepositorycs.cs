using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace InMemory
{
    public class UserRepository : IUserRepository
    {
        private readonly List<User> _users = new();

        public UserRepository()
        {
            // Тестовый пользователь
            _users.Add(new User
            {
                Id = 1,
                FirstName = "Тестовый",
                LastName = "Пользователь",
                Email = "test@example.com",
                Phone = "+79999999999",
                HashPassword = "password",
                SubActivation = false,
                RegistrationDate = DateTime.Now
            });
        }

        public List<User> GetAll()
        {
            return _users;
        }

        public User? GetById(int id)
        {
            return _users.FirstOrDefault(u => u.Id == id);
        }

        public User? GetByEmail(string email)
        {
            return _users.FirstOrDefault(u => u.Email == email);
        }

        public int Add(User user)
        {
            user.Id = _users.Count > 0 ? _users.Max(u => u.Id) + 1 : 1;
            _users.Add(user);
            return user.Id;
        }

        public bool Update(User user)
        {
            var existing = GetById(user.Id);
            if (existing == null)
                return false;

            existing.CopyFrom(user);
            return true;
        }

        public bool Delete(int id)
        {
            var user = GetById(id);
            if (user == null)
                return false;

            return _users.Remove(user);
        }

        public bool Authenticate(string email, string password)
        {
            var user = GetByEmail(email);
            return user != null && user.HashPassword == password;
        }
    }
}
