
using System;

namespace WKClientsImporter.Models
{
    public class Customer : BaseModel
    {
        public string DNI { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public DateTime Birthdate { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}
