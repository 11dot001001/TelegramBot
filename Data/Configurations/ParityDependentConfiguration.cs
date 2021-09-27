using Database.Data.Model;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Database.Data.Configurations
{
	internal sealed class ParityDependentConfiguration : IEntityTypeConfiguration<ParityDependentScheduleSubject>
    {
        public void Configure(EntityTypeBuilder<ParityDependentScheduleSubject> builder) => builder.HasBaseType<ScheduleField>();
    }
}
