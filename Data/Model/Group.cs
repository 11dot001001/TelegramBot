using System;
using System.Collections.Generic;

namespace Database.Data.Model
{
	public class Group
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public DateTime StartEducation { get; set; }

		public virtual ICollection<Student> Students { get; set; }
		public virtual ICollection<ScheduleField> ScheduleSubjects { get; set; }

		public bool Parity(DateTime dateTime)
		{
			return (((dateTime - StartEducation).Days + (int)StartEducation.DayOfWeek - 1) / 7) % 2 == 0 ? true : false;
		}
	}
}
