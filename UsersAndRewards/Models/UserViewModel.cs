using System;
using System.ComponentModel.DataAnnotations;

namespace UsersAndRewards.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Не указано имя")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина имени должна быть от 3 до 50 символов")]
        public string Name { get; set; }

        [DataType(DataType.Date)]
        public DateTime Birthdate { get; set; }

        public int Age { get => DateTime.Now.Year - Birthdate.Year; }

        public string PhotoUrl { get; set; }
    }
}
