using System;
using System.ComponentModel.DataAnnotations;

namespace UsersAndRewards.Models
{
    public class RewardViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Не указано название")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Не может быть меньше 3 или больше 50 символов")]
        [RegularExpression(@"^[a-zA-Z0-9_ \-]*$")]
        public string Title { get; set; }

        [MaxLength(250)]
        public string Description { get; set; }

        public string ImageUrl { get; set; }
    }
}
