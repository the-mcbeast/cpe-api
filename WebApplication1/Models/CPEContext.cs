using Microsoft.EntityFrameworkCore;
namespace CPEApi.Models
{
    public class CPEContext : DbContext
    {
        public CPEContext(DbContextOptions<CPEContext> options) : base(options)
        { }
        public DbSet<CPEItem> CPEItems { get; set; }
    }
}
