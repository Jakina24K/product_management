using Microsoft.EntityFrameworkCore;
using Server.model;

namespace Server.context
{
    public class ProductDb : DbContext
    {
        public ProductDb(DbContextOptions<ProductDb> options)
        : base(options)
        {

        }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<Session> UserSessions => Set<Session>();
        public DbSet<UserInfo> Users => Set<UserInfo>();
    }
}