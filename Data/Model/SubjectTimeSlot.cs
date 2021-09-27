using System;

namespace Database.Data.Model
{
    public class SubjectTimeSlot
    {
        protected SubjectTimeSlot() { }
        public SubjectTimeSlot(TimeSpan startLesson, TimeSpan endLesson, int order)
        {
            Id = Guid.NewGuid();
            Start = startLesson;
            End = endLesson;
            Order = order;
        }

        public Guid Id { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public int Order { get; set; }
    }
}
