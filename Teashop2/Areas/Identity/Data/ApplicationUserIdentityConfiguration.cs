using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Teashop2.Areas.Identity.Data;

namespace Teashop2.Data
{
    public class ApplicationUserIdentityConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(u => u.FirstName).HasMaxLength(25);
            builder.Property(u => u.LastName).HasMaxLength(30);
        }
    }
}