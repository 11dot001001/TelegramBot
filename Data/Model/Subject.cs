using System;
using System.Collections.Generic;

namespace Database.Data.Model
{
    public class Subject
    {
        protected Subject() { }
		public Subject(string name)
		{
            Id = Guid.NewGuid();
			Name = name;
		}

		public Guid Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<SubjectInstance> Instances { get; set; }
    }
}
