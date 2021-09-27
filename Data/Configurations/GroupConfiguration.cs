using Database.Data.Model;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Database.Data.Configurations
{
	internal class GroupConfiguration : IEntityTypeConfiguration<Group>
	{
		public void Configure(EntityTypeBuilder<Group> builder)
		{
			builder.HasKey(x => x.Id);
			builder.Property(x => x.Name).IsRequired();
			builder.Property(x => x.StartEducation).IsRequired();
			builder.HasMany(x => x.ScheduleSubjects).WithOne(x => x.Group).HasForeignKey(x => x.GroupId);
			builder.HasIndex(x => x.Name);
		}
	}
}
