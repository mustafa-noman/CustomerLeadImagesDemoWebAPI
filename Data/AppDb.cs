using LeadImagesDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace LeadImagesDemo.Data
{
    public class AppDb : DbContext
    {
        public AppDb(DbContextOptions<AppDb> options) : base(options) { }

        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Lead> Leads => Set<Lead>();
        public DbSet<ImageRecord> Images => Set<ImageRecord>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);
            b.Entity<ImageRecord>(e =>
            {
                e.Property(p => p.Base64Data).IsRequired();
                e.HasIndex(p => new { p.OwnerType, p.OwnerId, p.SequenceNo }).IsUnique();
                e.ToTable(t =>
                    t.HasCheckConstraint("CK_Image_Seq_1_10", "SequenceNo BETWEEN 1 AND 10"));
            });
        }
    }
}
