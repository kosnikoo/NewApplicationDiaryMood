using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string HashPassword { get; set; }
        public bool SubActivation { get; set; } = false;
        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        public void CopyFrom(User other)
        {
            FirstName = other.FirstName;
            LastName = other.LastName;
            Email = other.Email;
            Phone = other.Phone;
            HashPassword = other.HashPassword;
            SubActivation = other.SubActivation;
        }
    }
}
