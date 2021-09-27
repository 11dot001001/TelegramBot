using Database.Data.Model;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Database.Data.Configurations
{
	internal sealed class ScheduleFieldConfiguration : IEntityTypeConfiguration<ScheduleField>
    {
        public void Configure(EntityTypeBuilder<ScheduleField> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Order).IsRequired();
            builder.Property(x => x.DayOfWeek).IsRequired();
        }
    }
}
