using System;

namespace Database.Data.Model
{
    public class ParityDependentScheduleSubject : ScheduleField
    {
        protected ParityDependentScheduleSubject() { }
        public ParityDependentScheduleSubject(
            SubjectInstance numeratorSubjectInstance,
            SubjectInstance denominatorSubjectInstance,
            int order,
            DayOfWeek weekday,
             Guid groupId
        ) : base(order, weekday, groupId)
        {
            ParitySubjectInstanceId = numeratorSubjectInstance?.Id;
            NotParitySubjectInstanceId = denominatorSubjectInstance?.Id;
        }

        public Guid? ParitySubjectInstanceId { get; set; }
        public SubjectInstance ParitySubjectInstance { get; set; }
        public Guid? NotParitySubjectInstanceId { get; set; }
        public SubjectInstance NotParitySubjectInstance { get; set; }

        public SubjectInstance GetSubject(bool parity) => parity ? ParitySubjectInstance : NotParitySubjectInstance;
    }
}
