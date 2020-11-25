using System.Collections.Generic;

namespace UsersAndRewards.Database.Tables
{
    public class Reward
    {
        private List<User> _users;

        public int Id { get; set; }
        
        public string Title { get; set; }

        public string Description { get; set; }
        
        public string Image { get; set; }

        public List<User> Users
        {
            get
            {
                if (_users == null)
                    _users = new List<User>();
                return _users;
            }
            set => _users = value;
        }
    }
}
