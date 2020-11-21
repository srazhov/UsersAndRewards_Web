using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UsersAndRewards.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }
        
        public string Name { get; set; }

        [DataType(DataType.Date)]
        public DateTime Birthdate { get; set; }

        public int Age { get => DateTime.Now.Year - Birthdate.Year; }

        public string PhotoUrl { get; set; }
    }
}
