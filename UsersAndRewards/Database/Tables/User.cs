using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UsersAndRewards.Database.Tables
{
    public class User
    {
        private List<Reward> _rewards;

        public int Id { get; set; }

        public string Name { get; set; }

        [DataType(DataType.Date)]
        public DateTime Birthdate { get; set; }

        public string Photo { get; set; }

        public List<Reward> Rewards
        {
            get
            {
                if (_rewards == null)
                    _rewards = new List<Reward>();
                return _rewards;
            }
            set => _rewards = value;
        }
    }
}
