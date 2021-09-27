using System;

namespace Database.Data.Model
{
    public class ParityIndependentScheduleSubject : ScheduleField
    {
        protected ParityIndependentScheduleSubject() { }
		public ParityIndependentScheduleSubject(SubjectInstance subject, int order, DayOfWeek weekday, Guid groupId) 
            : base(order, weekday, groupId)
		{
            SubjectId = subject.Id;
		}

        public Guid SubjectId { get; set; }
        public SubjectInstance Subject { get; set; }
    }
}
