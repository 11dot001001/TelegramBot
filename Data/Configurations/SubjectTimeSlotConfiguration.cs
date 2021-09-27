using Database.Data.Model;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Database.Data.Configurations
{
	internal class SubjectTimeSlotConfiguration : IEntityTypeConfiguration<SubjectTimeSlot>
    {
        public void Configure(EntityTypeBuilder<SubjectTimeSlot> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Start).IsRequired();
            builder.Property(x => x.End).IsRequired();
            builder.Property(x => x.Order).IsRequired();
        }
    }
}
