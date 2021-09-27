using Database.Data;
using Database.Data.Model;
using Domain.Data;
using System;
using System.Collections.Generic;

namespace Domain
{
	public class ScheduleNotifier
	{
		private struct NotificationData
		{
			public DateTime DateTime;
			public int Order;

			public NotificationData(DateTime dateTime, int order)
			{
				DateTime = dateTime;
				Order = order;
			}
		}

		static private readonly TimeSpan BeforehandTime = new(0, 5, 0);
		static private readonly TimeSpan DeltaTime = TimeSpan.Zero;

		private readonly DataProvider _dataProvider;
		private readonly DataContext _dataContext;
		private readonly List<NotificationData> _startSubjectDateTimeNotifications;
		private readonly List<NotificationData> _endSubjectDateTimeNotifications;
		private DateTime _startToday;

		public ScheduleNotifier(DataProvider dataProvider, DataContext dataContext)
		{
			_dataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
			_dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));

			_startSubjectDateTimeNotifications = new List<NotificationData>();
			_endSubjectDateTimeNotifications = new List<NotificationData>();
		}

		public void Notify()
		{
			if (_dataProvider.CorrectedDateTime.Day != _startToday.Day)
				UpdateDateTimeNotifications();

			for (int i = 0; i != _startSubjectDateTimeNotifications.Count;)
			{
				if ((_dataProvider.CorrectedDateTime - _startSubjectDateTimeNotifications[i].DateTime) < DeltaTime)
					break;
				_dataProvider.StartSubjectDateTimeNotificate(_startSubjectDateTimeNotifications[i].Order);
				_startSubjectDateTimeNotifications.RemoveAt(i);
			}

			for (int i = 0; i != _endSubjectDateTimeNotifications.Count;)
			{
				if ((_dataProvider.CorrectedDateTime - _endSubjectDateTimeNotifications[i].DateTime) < DeltaTime)
					break;
				_dataProvider.EndSubjectDateTimeNotificate(_endSubjectDateTimeNotifications[i].Order);
				_endSubjectDateTimeNotifications.RemoveAt(i);
			}
		}
		private void UpdateDateTimeNotifications()
		{
			_dataContext.AddLog(_dataProvider.CorrectedDateTime + " " + _startToday);
			_startToday = _dataProvider.CorrectedDateTime.Date;
			_startSubjectDateTimeNotifications.Clear();
			_endSubjectDateTimeNotifications.Clear();
			List<SubjectTimeSlot> subjectTimeSlots = new(_dataContext.SubjectTimeSlots);

			foreach (SubjectTimeSlot timeSlot in subjectTimeSlots)
			{
				DateTime startLessonTime = _startToday.Add(timeSlot.Start - BeforehandTime);
				DateTime endLessonTime = _startToday.Add(timeSlot.End);

				if ((_dataProvider.CorrectedDateTime - startLessonTime) < DeltaTime)
					_startSubjectDateTimeNotifications.Add(new NotificationData(startLessonTime, timeSlot.Order));
				if ((_dataProvider.CorrectedDateTime - endLessonTime) < DeltaTime)
					_endSubjectDateTimeNotifications.Add(new NotificationData(endLessonTime, timeSlot.Order));
			}
		}
	}
}
