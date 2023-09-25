using IBDirect.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace IBDirect.API.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Users> Users { get; set; }

}
