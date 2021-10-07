using Data;
using Database.Data.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Database.Data
{
	public class DataContext : DbContext
	{
		public DataContext(DbContextOptions<DataContext> options) : base(options) 
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
			AneDataInitializer.EnsureDataInitialization(modelBuilder);
		}

		public DbSet<Student> Students { get; set; }
		public DbSet<Group> Groups { get; set; }
		public DbSet<SubjectTimeSlot> SubjectTimeSlots { get; set; }
		public DbSet<Subject> Subjects { get; set; }
		public DbSet<SubjectInstance> SubjectInstances { get; set; }
		public DbSet<ScheduleField> ScheduleFields { get; set; }
		public DbSet<ParityDependentScheduleSubject> ParityDependentScheduleSubjects { get; set; }
		public DbSet<ParityIndependentScheduleSubject> ParityIndependentScheduleSubjects { get; set; }

		public void CreateStudent(long telegramID, long chatID, string name, out Student student)
		{
			student = new Student
			{
				Id = Guid.NewGuid(),
				TelegramId = telegramID,
				ChatId = chatID,
				Name = name,
			};

			Students.Add(student);
			SaveChanges();
		}
		public bool TryGetStudentByTelegramId(long telegramId, out Student student)
		{
			student = Students.FirstOrDefault(x => x.TelegramId == telegramId);
			return student != null;
		}

		public void CreateGroup(string name, DateTime startEducation, Student student, out Group group)
		{
			group = new Group
			{
				Id = Guid.NewGuid(),
				Name = name,
				StartEducation = startEducation,
				Students = new Student[] { student }
			};
			Groups.Add(group);
			SaveChanges();
		}
		public Group GetGroupById(Guid id)
		{
			return Groups.Include(x => x.ScheduleSubjects).First(x => x.Id == id);
		}

		public void LoadAll()
		{
			Students.Load();
			Groups.Load();
			SubjectTimeSlots.Load();
			Subjects.Load();
			SubjectInstances.Load();
			ScheduleFields.Load();
			ParityDependentScheduleSubjects.Load();
			ParityIndependentScheduleSubjects.Load();
		}
	}
}
