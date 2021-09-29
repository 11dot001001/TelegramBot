using Database.Data.Model;
using Microsoft.EntityFrameworkCore;
using System;

namespace Data
{
	public static class AneDataInitializer
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
			},
			new Student
			{
				Id = new("0CDB67A9-8B2A-426F-9529-2A7E66D960E6"),
				TelegramId = 661200954,
				ChatId = 661200954,
				Name = "ane0503",
				GroupId = GroupId
			},
		};

		private static Guid GroupId = new("74938781-7D88-429C-80EF-84A531A55883");
		private static Group[] Groups => new[]
		{
			new Group
			{
				Id = GroupId,
				Name = "Б-ЛПеФА-21",
				StartEducation = new DateTime(2021, 9, 1),
			}
		};
		private static SubjectTimeSlot[] SubjectTimeSlots => new[]
		{
			new SubjectTimeSlot(new TimeSpan(8, 30, 0), new TimeSpan(10, 05, 0), 0),
			new SubjectTimeSlot(new TimeSpan(10, 25, 0), new TimeSpan(12, 00, 0), 1),
			new SubjectTimeSlot(new TimeSpan(12, 20, 0), new TimeSpan(13, 55, 0), 2),
			new SubjectTimeSlot(new TimeSpan(14, 10, 0), new TimeSpan(15, 45, 0), 3),
			new SubjectTimeSlot(new TimeSpan(15, 55, 0), new TimeSpan(17, 30, 0), 4),
			new SubjectTimeSlot(new TimeSpan(17, 40, 0), new TimeSpan(19, 15, 0), 5)
		};

		//(Инд) - индивидуальная
		//(Лб) - Лабы
		//(Пр) - Практикум
		//(Лк) - Лекции

		private static readonly Subject GrammaticalSpeechAspects = new("Грамматические аспекты речи на первом иностранном языке");
		private static readonly Subject Logic = new("Логика");
		private static readonly Subject ProjectActivityFundamentals = new("Основы проектной деятельности");
		private static readonly Subject TheoreticalLinguisticsFundamentals = new("Основы теоретической лингвистики");
		private static readonly Subject FirstForeignPracticalCourse = new("Практический курс первого иностранного языка");
		private static readonly Subject SecondForeignPracticalCourse = new("Практический курс второго иностранного языка");
		private static readonly Subject NetworkSocialServices = new("Сетевые социальные сервисы в профессиональной деятельности");
		private static readonly Subject ArtLanguage = new("Язык искусства (великие книги, великие фильмы, музыка, живопись)");
		private static readonly Subject LifeSafety = new("Безопасность жизнедеятельности");
		private static readonly Subject UrbanStudies = new("Урбанистика");

		private static Subject[] Subjects => new[]
		{
			GrammaticalSpeechAspects,
			Logic,
			ProjectActivityFundamentals,
			TheoreticalLinguisticsFundamentals,
			FirstForeignPracticalCourse,
			SecondForeignPracticalCourse,
			NetworkSocialServices,
			ArtLanguage,
			LifeSafety,
			UrbanStudies
		};

		private static readonly SubjectInstance GrammaticalSpeechAspects_1 = new(SubjectType.Laboratory, "41 к.5", "Чиликина О. Н.", GrammaticalSpeechAspects);
		private static readonly SubjectInstance GrammaticalSpeechAspects_2 = new(SubjectType.Laboratory, "39 к.5", "Чиликина О. Н.", GrammaticalSpeechAspects);

		private static readonly SubjectInstance LogicPracticum = new(SubjectType.Practicum, "9 а к.5", "Федяй И. В.", Logic);
		private static readonly SubjectInstance LogicIndividual = new(SubjectType.Individual, "9 а к.5", "Федяй И. В.", Logic);

		private static readonly SubjectInstance ProjectActivityFundamentalsPracticum = new(SubjectType.Practicum, "9 а к.5", "Кондрашова Н. Г.", ProjectActivityFundamentals);
		private static readonly SubjectInstance ProjectActivityFundamentalsIndividual = new(SubjectType.Individual, "9 а к.5", "Кондрашова Н. Г.", ProjectActivityFundamentals);

		private static readonly SubjectInstance TheoreticalLinguisticsFundamentals_1 = new(SubjectType.Practicum, "9 а к.5", "Исаева М. С.", TheoreticalLinguisticsFundamentals);
		private static readonly SubjectInstance TheoreticalLinguisticsFundamentals_2 = new(SubjectType.Practicum, "11 к.5", "Исаева М. С.", TheoreticalLinguisticsFundamentals);
		private static readonly SubjectInstance TheoreticalLinguisticsFundamentalsLecture = new(SubjectType.Lecture, "5 к.5", "Исаева М. С.", TheoreticalLinguisticsFundamentals);

		private static readonly SubjectInstance FirstForeignPracticalCourse_Lb_22 = new(SubjectType.Laboratory, "22 к.5", "Чиликина О. Н.", FirstForeignPracticalCourse);
		private static readonly SubjectInstance FirstForeignPracticalCourse_Lb_20 = new(SubjectType.Laboratory, "20 к.5", "Чиликина О. Н.", FirstForeignPracticalCourse);
		private static readonly SubjectInstance FirstForeignPracticalCourse_Lb_23 = new(SubjectType.Laboratory, "23 к.5", "Чиликина О. Н.", FirstForeignPracticalCourse);
		private static readonly SubjectInstance FirstForeignPracticalCourse_Lb_11 = new(SubjectType.Laboratory, "11 к.5", "Осипова Т. И.", FirstForeignPracticalCourse);

		private static readonly SubjectInstance SecondForeignPracticalCourse_Lb_23 = new(SubjectType.Laboratory, "23 к.5", "Сергина К. И.", SecondForeignPracticalCourse);
		private static readonly SubjectInstance SecondForeignPracticalCourse_Lb_25 = new(SubjectType.Laboratory, "25 к.5", "Сергина К. И.", SecondForeignPracticalCourse);

		private static readonly SubjectInstance NetworkSocialServicesPracticum = new(SubjectType.Practicum, "709 к.1", "Столярова Н. Б.", NetworkSocialServices);

		private static readonly SubjectInstance ArtLanguageIndividual = new(SubjectType.Individual, "10 к.5", "Заборина М. А.", ArtLanguage);

		private static readonly SubjectInstance LifeSafetyPracticum = new(SubjectType.Practicum, "13 к.5", "Лисовская Л. П.", LifeSafety);
		private static readonly SubjectInstance LifeSafetyLecture = new(SubjectType.Lecture, "310 к.1", "Лисовская Л. П.", LifeSafety);

		private static readonly SubjectInstance UrbanStudiesLecture = new(SubjectType.Lecture, "310 к.1", "Кабанов К. В.", UrbanStudies);

		private static SubjectInstance[] SubjectInstances => new[]
		{
			GrammaticalSpeechAspects_1,
			GrammaticalSpeechAspects_2,
			LogicPracticum,
			LogicIndividual,
			ProjectActivityFundamentalsPracticum,
			ProjectActivityFundamentalsIndividual,
			TheoreticalLinguisticsFundamentals_1,
			TheoreticalLinguisticsFundamentals_2,
			TheoreticalLinguisticsFundamentalsLecture,
			FirstForeignPracticalCourse_Lb_22,
			FirstForeignPracticalCourse_Lb_20,
			FirstForeignPracticalCourse_Lb_23,
			FirstForeignPracticalCourse_Lb_11,
			SecondForeignPracticalCourse_Lb_23,
			SecondForeignPracticalCourse_Lb_25,
			NetworkSocialServicesPracticum,
			ArtLanguageIndividual,
			LifeSafetyPracticum,
			LifeSafetyLecture,
			UrbanStudiesLecture,
		};

		//Monday
		private static readonly ParityDependentScheduleSubject mondey0 = new(null, GrammaticalSpeechAspects_1, 0, DayOfWeek.Monday, GroupId);
		private static readonly ParityDependentScheduleSubject mondey1 = new(LogicPracticum, ProjectActivityFundamentalsPracticum, 1, DayOfWeek.Monday, GroupId);
		private static readonly ParityDependentScheduleSubject mondey2 = new(TheoreticalLinguisticsFundamentals_1, TheoreticalLinguisticsFundamentals_2, 2, DayOfWeek.Monday, GroupId);
		private static readonly ParityDependentScheduleSubject mondey3 = new(FirstForeignPracticalCourse_Lb_22, null, 3, DayOfWeek.Monday, GroupId);

		//Tuesday
		private static readonly ParityIndependentScheduleSubject tuesday0 = new(SecondForeignPracticalCourse_Lb_23, 0, DayOfWeek.Tuesday, GroupId);
		private static readonly ParityDependentScheduleSubject tuesday1 = new(NetworkSocialServicesPracticum, ArtLanguageIndividual, 1, DayOfWeek.Tuesday, GroupId);

		//Wednesday
		private static readonly ParityDependentScheduleSubject wednesday0 = new(LogicIndividual, null, 0, DayOfWeek.Wednesday, GroupId);
		private static readonly ParityDependentScheduleSubject wednesday1 = new(FirstForeignPracticalCourse_Lb_20, FirstForeignPracticalCourse_Lb_23, 1, DayOfWeek.Wednesday, GroupId);
		private static readonly ParityIndependentScheduleSubject wednesday2 = new(LifeSafetyPracticum, 2, DayOfWeek.Wednesday, GroupId);

		//Thursday
		private static readonly ParityDependentScheduleSubject thursday0 = new(null, ProjectActivityFundamentalsIndividual, 0, DayOfWeek.Thursday, GroupId);
		private static readonly ParityIndependentScheduleSubject thursday1 = new(SecondForeignPracticalCourse_Lb_25, 1, DayOfWeek.Thursday, GroupId);
		private static readonly ParityIndependentScheduleSubject thursday2 = new(TheoreticalLinguisticsFundamentalsLecture, 2, DayOfWeek.Thursday, GroupId);

		//Friday
		private static readonly ParityDependentScheduleSubject friday0 = new(GrammaticalSpeechAspects_2, LifeSafetyLecture, 0, DayOfWeek.Friday, GroupId);
		private static readonly ParityIndependentScheduleSubject friday1 = new(FirstForeignPracticalCourse_Lb_11, 1, DayOfWeek.Friday, GroupId);
		private static readonly ParityIndependentScheduleSubject friday2 = new(UrbanStudiesLecture, 2, DayOfWeek.Friday, GroupId);

		private static ParityIndependentScheduleSubject[] ParityIndependentScheduleFields => new[]
		{
			tuesday0,
			wednesday2,
			thursday1,
			thursday2,
			friday1,
			friday2,
		};
		private static ParityDependentScheduleSubject[] ParityDependentScheduleFields => new ParityDependentScheduleSubject[]
		{
			mondey0,
			mondey1,
			mondey2,
			mondey3,
			tuesday1,
			wednesday0,
			wednesday1,
			thursday0,
			friday0,
		};
	}
}
