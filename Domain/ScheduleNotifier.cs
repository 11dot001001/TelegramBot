using Database.Data;
using Database.Data.Model;
using Domain.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Domain
{
	public class ScheduleNotifier
	{
		private class NotificationData
		{
			public NotificationData(DateTime dateTime, int order)
			{
				DateTime = dateTime;
				Order = order;
			}

			public DateTime DateTime { get; set; }
			public int Order { get; set; }
			public bool IsNotified { get; set; }
		}

		static private readonly TimeSpan BeforehandTime = new(0, 5, 0);
		static private readonly TimeSpan DeltaTime = TimeSpan.Zero;

		private readonly DataProvider _dataProvider;
		private readonly DataContext _dataContext;
		private readonly ILogger<ScheduleNotifier> _logger;
		private readonly List<NotificationData> _startSubjectDateTimeNotifications;
		private readonly List<NotificationData> _endSubjectDateTimeNotifications;
		private DateTime _startToday;

		public ScheduleNotifier(DataProvider dataProvider, DataContext dataContext, ILogger<ScheduleNotifier> logger)
		{
			_dataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
			_dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));

			_startSubjectDateTimeNotifications = new List<NotificationData>();
			_endSubjectDateTimeNotifications = new List<NotificationData>();
		}

		public void Notify()
		{
			if (_dataProvider.CorrectedDateTime.Day != _startToday.Day)
				UpdateDateTimeNotifications();

			NotifyTimeSlotBound(_startSubjectDateTimeNotifications, _dataProvider.StartSubjectDateTimeNotificate);
			NotifyTimeSlotBound(_endSubjectDateTimeNotifications, _dataProvider.EndSubjectDateTimeNotificate);
		}

		private delegate void NotifySubjectTimeSlot(int order);
		private void NotifyTimeSlotBound(IEnumerable<NotificationData> notifications, NotifySubjectTimeSlot notify)
		{
			foreach (NotificationData notificationData in notifications)
			{
				if (notificationData.IsNotified)
					continue;

				if (_dataProvider.CorrectedDateTime < notificationData.DateTime)
					continue;

				notify(notificationData.Order);
				notificationData.IsNotified = true;
			}
		}
		
		private void UpdateDateTimeNotifications()
		{
			_logger.LogInformation($"" +
				$"Invoked {nameof(UpdateDateTimeNotifications)}. " +
				$"{nameof(_dataProvider.CorrectedDateTime)}: {_dataProvider.CorrectedDateTime}. " +
				$"{nameof(_startToday)}: {_startToday}. "
			);
			_startToday = _dataProvider.CorrectedDateTime.Date;
			_startSubjectDateTimeNotifications.Clear();
			_endSubjectDateTimeNotifications.Clear();
			List<SubjectTimeSlot> subjectTimeSlots = new(_dataContext.SubjectTimeSlots);

			foreach (SubjectTimeSlot timeSlot in subjectTimeSlots)
			{
				DateTime startLessonTime = _startToday.Add(timeSlot.Start - BeforehandTime);
				DateTime endLessonTime = _startToday.Add(timeSlot.End);

				if ((_dataProvider.CorrectedDateTime - startLessonTime) < DeltaTime)
				{
					_startSubjectDateTimeNotifications.Add(new NotificationData(startLessonTime, timeSlot.Order));
					_logger.LogInformation($"Added daily start timeSlot notification at {startLessonTime}.");
				}
				if ((_dataProvider.CorrectedDateTime - endLessonTime) < DeltaTime)
				{
					_endSubjectDateTimeNotifications.Add(new NotificationData(endLessonTime, timeSlot.Order));
					_logger.LogInformation($"Added daily end timeSlot notification at {endLessonTime}.");
				}
			}
		}
	}
}
