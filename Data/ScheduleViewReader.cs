using Database.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Data
{
	static public class ScheduleViewReader
	{
		private const int _weekDayCount = 7;

		static public string GetWeekDayString(DayOfWeek dayOfWeek)
		{
			return dayOfWeek switch
			{
				DayOfWeek.Friday => "Пятница",
				DayOfWeek.Monday => "Понедельник",
				DayOfWeek.Saturday => "Суббота",
				DayOfWeek.Sunday => "Воскресенье",
				DayOfWeek.Thursday => "Четверг",
				DayOfWeek.Tuesday => "Вторник",
				DayOfWeek.Wednesday => "Среда",
				_ => string.Empty,
			};
		}
		static public string GetSubjectTypeString(SubjectType subjectType)
		{
			return subjectType switch
			{
				SubjectType.Lecture => "(Лк)",
				SubjectType.Laboratory => "(Лб)",
				SubjectType.Seminar => "Семинар",
				SubjectType.Exercise => "Упражнение",
				SubjectType.Coursework => "Курсовая",
				SubjectType.ScientificResearch => "Научная работа",
				SubjectType.Individual => "(Инд)",
				SubjectType.Practicum => "(Пр)",
				_ => string.Empty,
			};
		}

		static public bool GetSchedule(Group group, out string schedule)
		{
			schedule = string.Empty;
			if (group == null || group.ScheduleSubjects == null)
				return false;

			for (int day = (int)DayOfWeek.Monday; day < _weekDayCount; day++)
			{
				List<ScheduleView> scheduleViewItems = GetSchedule(group, (DayOfWeek)day);
				if (scheduleViewItems == null)
					continue;
				schedule += "*" + GetWeekDayString((DayOfWeek)day) + "*\n";

				foreach (ScheduleView subject in scheduleViewItems)
				{
					string paritySring = string.Empty;
					if (subject.ParityType.HasValue)
					{
						paritySring = subject.ParityType.Value == ScheduleView.ParityTypeEnum.Parity ? "(числитель) " : "(знаменатель) ";
					}

					schedule +=
					@"`    " + (subject.Order + 1).ToString() + ". " +
					paritySring +
					subject.SubjectInstance.Subject.Name + " " +
					subject.SubjectInstance.Audience + " " +
					subject.SubjectInstance.Teacher + " " +
					GetSubjectTypeString(subject.SubjectInstance.SubjectType) + "`\n";
				}
				schedule += "\n";
			}
			return true;
		}
		static public bool GetSchedule(Group group, DateTime dateTime, List<SubjectTimeSlot> subjectCalls, out string schedule)
		{
			schedule = string.Empty;
			if (group == null || group.ScheduleSubjects == null || subjectCalls == null)
				return false;

			schedule = "*" + GetWeekDayString(dateTime.DayOfWeek) + "*\n\n";

			List<ScheduleView> scheduleViewItems = GetSchedule(group, dateTime.DayOfWeek, group.Parity(dateTime));
			if (scheduleViewItems == null)
				return true;

			foreach (ScheduleView subject in scheduleViewItems)
			{
				schedule +=
				"`" + (subject.Order + 1).ToString() + ". " + subject.SubjectInstance.Subject.Name + "`\n" +
				"▸ `" + GetSubjectTypeString(subject.SubjectInstance.SubjectType) + "`\n" +
				"▸ `" + subject.SubjectInstance.Audience + "`\n" +
				"▸ `" + subject.SubjectInstance.Teacher + "`\n" +
				"▸ `" + subjectCalls[subject.Order].Start.ToString(@"hh\:mm") + " - " + subjectCalls[subject.Order].End.ToString(@"hh\:mm") + "`\n\n";
			}
			return true;
		}

		static public List<ScheduleView> GetSchedule(Group group, DayOfWeek dayOfWeek, bool parity)
		{
			List<ScheduleField> dayScheduleFields = group.ScheduleSubjects.Where(x => x.DayOfWeek == dayOfWeek).ToList();
			if (dayScheduleFields.Count == 0)
				return null;

			List<ScheduleView> scheduleViewItems = new();

			dayScheduleFields.Sort(new Comparison<ScheduleField>((x, y) => x.Order.CompareTo(y.Order)));
			foreach (ScheduleField subject in dayScheduleFields)
			{
				if (subject is ParityDependentScheduleSubject parityDependentSubject)
				{
					SubjectInstance subjectInstance = parityDependentSubject.GetSubject(parity);
					if (subjectInstance != null)
						scheduleViewItems.Add(new ScheduleView(parityDependentSubject.Order, subjectInstance));
					continue;
				}
				if (subject is ParityIndependentScheduleSubject parityIndependentSubject)
				{
					scheduleViewItems.Add(new ScheduleView(parityIndependentSubject.Order, parityIndependentSubject.Subject));
					continue;
				}
			}
			return scheduleViewItems;
		}
		static private List<ScheduleView> GetSchedule(Group group, DayOfWeek dayOfWeek)
		{
			List<ScheduleField> dayScheduleFields = group.ScheduleSubjects.Where(x => x.DayOfWeek == dayOfWeek).ToList();
			if (dayScheduleFields.Count == 0)
				return null;

			List<ScheduleView> scheduleViewItems = new();

			dayScheduleFields.Sort(new Comparison<ScheduleField>((x, y) => x.Order.CompareTo(y.Order)));
			foreach (ScheduleField subject in dayScheduleFields)
			{
				if (subject is ParityDependentScheduleSubject parityDependentSubject)
				{
					if (parityDependentSubject.ParitySubjectInstance != null)
						scheduleViewItems.Add(new ScheduleView(parityDependentSubject.Order, parityDependentSubject.ParitySubjectInstance, ScheduleView.ParityTypeEnum.Parity));
					if (parityDependentSubject.NotParitySubjectInstance != null)
						scheduleViewItems.Add(new ScheduleView(parityDependentSubject.Order, parityDependentSubject.NotParitySubjectInstance, ScheduleView.ParityTypeEnum.NotParity));
					continue;
				}
				if (subject is ParityIndependentScheduleSubject parityIndependentSubject)
				{
					scheduleViewItems.Add(new ScheduleView(parityIndependentSubject.Order, parityIndependentSubject.Subject));
					continue;
				}
			}
			return scheduleViewItems;
		}
	}

	public class ScheduleView
	{
		public enum ParityTypeEnum
		{
			Parity,
			NotParity
		}

		public int Order;
		public SubjectInstance SubjectInstance;
		public ParityTypeEnum? ParityType;

		public ScheduleView(int order, SubjectInstance subjectInstance, ParityTypeEnum? parityType = null)
		{
			Order = order;
			SubjectInstance = subjectInstance ?? throw new ArgumentNullException(nameof(subjectInstance));
			ParityType = parityType;
		}
	}
}
