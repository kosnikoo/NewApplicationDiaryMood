using Domain;
using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface
{
    public interface IUserRepository
    {
        List<User> GetAll();
        User GetById(int id);
        User GetByEmail(string email);
        int Add(User user);
        bool Update(User user);
        bool Delete(int id);
        bool Authenticate(string email, string password);
    }
}
