using System;
using System.ComponentModel.DataAnnotations;

namespace UsersAndRewards.Database.Tables
{
    public class User
    {
        public int Id { get; set; }
        
        public string Name { get; set; }

        [DataType(DataType.Date)]
        public DateTime Birthdate { get; set; }

        public string Photo { get; set; }
    }
}
