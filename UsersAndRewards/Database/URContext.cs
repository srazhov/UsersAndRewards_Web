using Microsoft.EntityFrameworkCore;
using UsersAndRewards.Database.Tables;

namespace UsersAndRewards.Database
{
    public class URContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        
        public DbSet<Reward> Rewards { get; set; }
        
        public URContext(DbContextOptions options) : base(options) { }
    }
}
