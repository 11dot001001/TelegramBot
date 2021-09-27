using Database.Data.Model;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Database.Data.Configurations
{
	internal sealed class SubjectInstanceConfiguration : IEntityTypeConfiguration<SubjectInstance>
    {
        public void Configure(EntityTypeBuilder<SubjectInstance> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.SubjectType).IsRequired();
            builder.Property(x => x.Audience).IsRequired();
            builder.Property(x => x.Teacher).IsRequired();
        }
    }
}
