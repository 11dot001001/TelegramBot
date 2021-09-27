using Database.Data.Model;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Database.Data.Configurations
{
	internal sealed class ParityIndependentConfiguration : IEntityTypeConfiguration<ParityIndependentScheduleSubject>
    {
		public void Configure(EntityTypeBuilder<ParityIndependentScheduleSubject> builder)
		{
			builder.HasBaseType<ScheduleField>();
		}
	}
}
