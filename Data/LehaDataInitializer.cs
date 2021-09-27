using Database.Data.Model;
using Microsoft.EntityFrameworkCore;
using System;

namespace Data
{
	public static class LehaDataInitializer
	{
		public static void EnsureDataInitialization(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Group>().HasData(Groups);
			modelBuilder.Entity<Student>().HasData(Students);
			modelBuilder.Entity<SubjectTimeSlot>().HasData(SubjectTimeSlots);
			modelBuilder.Entity<Subject>().HasData(Subjects);
			modelBuilder.Entity<SubjectInstance>().HasData(SubjectInstances);
			modelBuilder.Entity<ParityIndependentScheduleSubject>().HasData(ParityIndependentScheduleFields);
			modelBuilder.Entity<ParityDependentScheduleSubject>().HasData(ParityDependentScheduleFields);
		}

		private static Student[] Students => new[]
		{
			new Student
			{
				Id = new("74938781-7D88-429C-80EF-84A531A55889"),
				TelegramId = 399105658,
				ChatId = 399105658,
				Name = "alexseychik (Alexsey Chuvilkin)",
				GroupId = GroupId
			}
		};

		private static Guid GroupId = new("74938781-7D88-429C-80EF-84A531A55882");
		private static Group[] Groups => new[]
		{
			new Group
			{
				Id = GroupId,
				Name = "ЭВМ.Б-61",
				StartEducation = new DateTime(2019, 2, 7),
			}
		};
		private static SubjectTimeSlot[] SubjectTimeSlots => new[]
		{
			new SubjectTimeSlot(new TimeSpan(8, 30, 0), new TimeSpan(10, 05, 0), 0),
			new SubjectTimeSlot(new TimeSpan(10, 20, 0), new TimeSpan(11, 55, 0), 1),
			new SubjectTimeSlot(new TimeSpan(12, 10, 0), new TimeSpan(13, 45, 0), 2),
			new SubjectTimeSlot(new TimeSpan(14, 15, 0), new TimeSpan(15, 50, 0), 3),
			new SubjectTimeSlot(new TimeSpan(16, 05, 0), new TimeSpan(17, 40, 0), 4),
			new SubjectTimeSlot(new TimeSpan(17, 50, 0), new TimeSpan(19, 25, 0), 5)
		};

		private static readonly Subject Modeling = new("Моделирование");
		private static readonly Subject MicroprocessorSystem = new("Микропроцессорные системы");
		private static readonly Subject IbmArchitecture = new("Архитектура ЭВМ");
		private static readonly Subject Ibm = new("ЭВМ");
		private static readonly Subject PhysicalCulture = new("Физкультура");
		private static readonly Subject Russian = new("Русский язык и культура речи");
		private static readonly Subject Economy = new("Экономика");
		private static readonly Subject Jurisprudence = new("Правоведение");
		private static readonly Subject Circuitry = new("Схемотехника");
		private static readonly Subject Foreign = new("ИНО");
		private static readonly Subject LifeSafety = new("БЖД");
		private static readonly Subject ResearchWork = new("НИР");
		private static Subject[] Subjects => new[]
		{
			Modeling,
			MicroprocessorSystem,
			IbmArchitecture,
			Ibm,
			PhysicalCulture,
			Russian,
			Economy,
			Jurisprudence,
			Circuitry,
			Foreign,
			LifeSafety,
			ResearchWork
		};

		private static readonly SubjectInstance ModelingLecture = new(SubjectType.Lecture, "6-102", "Донецков", Modeling);
		private static readonly SubjectInstance modelingLaboratory = new(SubjectType.Laboratory, "3-218", "Донецков", Modeling);
		private static readonly SubjectInstance microprocessorSystemLecture = new(SubjectType.Lecture, "3-309", "Николаев", MicroprocessorSystem);
		private static readonly SubjectInstance microprocessorSystemExercise = new(SubjectType.Exercise, "3-309", "Николаев", MicroprocessorSystem);
		private static readonly SubjectInstance microprocessorSystemLaboratory = new(SubjectType.Laboratory, "3-309", "Николаев", MicroprocessorSystem);
		private static readonly SubjectInstance ibmArchitectureLecture = new(SubjectType.Lecture, "3-325", "Онуфриева", IbmArchitecture);
		private static readonly SubjectInstance ibmArchitectureLaboratory = new(SubjectType.Laboratory, "3-318", "Онуфриева", IbmArchitecture);
		private static readonly SubjectInstance ibmLecture = new(SubjectType.Lecture, "3-325", "Онуфриева", Ibm);
		private static readonly SubjectInstance ibmExercise = new(SubjectType.Exercise, "3-215", "Онуфриева", Ibm);
		private static readonly SubjectInstance ibmLaboratory = new(SubjectType.Laboratory, "3-318", "Онуфриева", Ibm);
		private static readonly SubjectInstance physicalCultureExercise = new(SubjectType.Exercise, "к.2", "-", PhysicalCulture);
		private static readonly SubjectInstance russianExercise = new(SubjectType.Exercise, "3-326", "Логинова", Russian);
		private static readonly SubjectInstance economyLecture = new(SubjectType.Lecture, "3-313", "Яловенко", Economy);
		private static readonly SubjectInstance economyExercise = new(SubjectType.Exercise, "3-313", "Яловенко", Economy);
		private static readonly SubjectInstance jurisprudenceLecture = new(SubjectType.Lecture, "3-403", "Шафигуллина", Jurisprudence);
		private static readonly SubjectInstance jurisprudenceExercise = new(SubjectType.Exercise, "3-313", "Шафигуллина", Jurisprudence);
		private static readonly SubjectInstance circuitryCoursework = new(SubjectType.Coursework, "к.3", "Максимов", Circuitry);
		private static readonly SubjectInstance foreignExercise = new(SubjectType.Exercise, "к.1", "Тунанова", Foreign);
		private static readonly SubjectInstance lifeSafetyLecture = new(SubjectType.Lecture, "7-309", "Никулина", LifeSafety);
		private static readonly SubjectInstance lifeSafetyExercise = new(SubjectType.Exercise, "7-305", "Фатеева", LifeSafety);
		private static readonly SubjectInstance lifeSafetyLaboratory = new(SubjectType.Laboratory, "7-307", "Фатеева", LifeSafety);
		private static readonly SubjectInstance researchWorkScientificResearch = new(SubjectType.ScientificResearch, "к.3", "-", ResearchWork);
		private static SubjectInstance[] SubjectInstances => new[]
		{
			ModelingLecture,
			modelingLaboratory,
			microprocessorSystemLecture,
			microprocessorSystemExercise,
			microprocessorSystemLaboratory,
			ibmArchitectureLecture ,
			ibmArchitectureLaboratory,
			ibmLecture,
			ibmExercise,
			ibmLaboratory,
			physicalCultureExercise,
			russianExercise,
			economyLecture ,
			economyExercise,
			jurisprudenceLecture,
			jurisprudenceExercise,
			circuitryCoursework,
			foreignExercise,
			lifeSafetyLecture ,
			lifeSafetyExercise,
			lifeSafetyLaboratory ,
			researchWorkScientificResearch
		};

		private static readonly ParityIndependentScheduleSubject mondey0 = new(ModelingLecture, 0, DayOfWeek.Monday, GroupId);
		private static readonly ParityDependentScheduleSubject mondey1 = new(microprocessorSystemExercise, ibmArchitectureLaboratory, 1, DayOfWeek.Monday, GroupId);
		private static readonly ParityDependentScheduleSubject mondey2 = new(ibmLecture, ibmArchitectureLecture, 2, DayOfWeek.Monday, GroupId);
		private static readonly ParityIndependentScheduleSubject mondey3 = new(physicalCultureExercise, 3, DayOfWeek.Monday, GroupId);
		private static readonly ParityDependentScheduleSubject tuesday0 = new(russianExercise, ibmExercise, 0, DayOfWeek.Tuesday, GroupId);
		private static readonly ParityIndependentScheduleSubject tuesday1 = new(economyLecture, 1, DayOfWeek.Tuesday, GroupId);
		private static readonly ParityIndependentScheduleSubject tuesday2 = new(jurisprudenceLecture, 2, DayOfWeek.Tuesday, GroupId);
		private static readonly ParityDependentScheduleSubject tuesday3 = new(economyExercise, jurisprudenceExercise, 3, DayOfWeek.Tuesday, GroupId);
		private static readonly ParityIndependentScheduleSubject wednesday0 = new(circuitryCoursework, 0, DayOfWeek.Wednesday, GroupId);
		private static readonly ParityIndependentScheduleSubject wednesday1 = new(foreignExercise, 1, DayOfWeek.Wednesday, GroupId);
		private static readonly ParityIndependentScheduleSubject wednesday2 = new(ibmLaboratory, 2, DayOfWeek.Wednesday, GroupId);
		private static readonly ParityIndependentScheduleSubject wednesday3 = new(physicalCultureExercise, 3, DayOfWeek.Wednesday, GroupId);
		private static readonly ParityIndependentScheduleSubject friday0 = new(microprocessorSystemLaboratory, 0, DayOfWeek.Friday, GroupId);
		private static readonly ParityIndependentScheduleSubject friday1 = new(lifeSafetyLecture, 1, DayOfWeek.Friday, GroupId);
		private static readonly ParityIndependentScheduleSubject friday2 = new(modelingLaboratory, 2, DayOfWeek.Friday, GroupId);
		private static readonly ParityIndependentScheduleSubject friday3 = new(researchWorkScientificResearch, 3, DayOfWeek.Friday, GroupId);
		private static readonly ParityIndependentScheduleSubject saturday0 = new(microprocessorSystemLecture, 1, DayOfWeek.Saturday, GroupId);
		private static readonly ParityDependentScheduleSubject saturday1 = new(lifeSafetyExercise, lifeSafetyLaboratory, 2, DayOfWeek.Saturday, GroupId);
		private static ParityIndependentScheduleSubject[] ParityIndependentScheduleFields => new[]
		{
			mondey0,
			mondey3,
			tuesday1,
			tuesday2,
			wednesday0,
			wednesday1,
			wednesday2,
			wednesday3,
			friday0,
			friday1,
			friday2,
			friday3,
			saturday0,
		};
		private static ParityDependentScheduleSubject[] ParityDependentScheduleFields => new ParityDependentScheduleSubject[]
		{
			mondey1,
			mondey2,
			tuesday0,
			tuesday3,
			saturday1
		};
	}
}
