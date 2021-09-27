using System;

namespace Database.Data.Model
{
    public abstract class ScheduleField
    {
        protected ScheduleField() { }
        protected ScheduleField(int order, DayOfWeek weekday, Guid groupId)
        {
            Id = Guid.NewGuid();
            Order = order;
            DayOfWeek = weekday;
            GroupId = groupId;
        }

        public Guid Id { get; set; }
        public int Order { get; set; }
        public DayOfWeek DayOfWeek { get; set; }

        public Guid GroupId { get; set; }
        public virtual Group Group { get; set; }
    }
}
