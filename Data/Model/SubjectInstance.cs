using System;
using System.Security.Principal;

namespace Database.Data.Model
{
    public class SubjectInstance
    {
        protected SubjectInstance() { }

        public SubjectInstance(SubjectType subjectType, string audience, string teacher, Subject subject)
        {
            Id = Guid.NewGuid();
            SubjectType = subjectType;
            Audience = audience ?? throw new ArgumentNullException(nameof(audience));
            Teacher = teacher ?? throw new ArgumentNullException(nameof(teacher));
            SubjectId = subject.Id;
        }

        public Guid Id { get; set; }
        public SubjectType SubjectType { get; set; }
        public string Audience { get; set; }
        public string Teacher { get; set; }
        public Guid SubjectId { get; set; }
        public Subject Subject { get; set; }
        public ScheduleField ScheduleSubject { get; set; }
    }
}
