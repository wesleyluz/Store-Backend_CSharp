using Microsoft.EntityFrameworkCore; 

namespace lojinha_backend.Model.Context
{
    public class MSSQLContext: DbContext
    {
        public MSSQLContext() { }

        public MSSQLContext(DbContextOptions<MSSQLContext> options) : base(options) { }

        public DbSet<Produto> produtos { get; set; }
        public DbSet<User> users { get; set; }

    }
}
