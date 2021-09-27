using Database.Data.Model;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Database.Data.Configurations
{
	internal class StudentConfiguration : IEntityTypeConfiguration<Student>
	{
		public void Configure(EntityTypeBuilder<Student> builder)
		{
			builder.HasKey(x => x.Id);
			builder.Property(x => x.TelegramId).IsRequired();
			builder.Property(x => x.ChatId).IsRequired();
			builder.Property(x => x.Name).IsRequired();

			builder.HasOne(x => x.Group).WithMany(x => x.Students).HasForeignKey(x => x.GroupId);
			builder.HasIndex(x => x.TelegramId).IsUnique();
		}
	}
}
