using System;

namespace Database.Data.Model
{
	public class Student
    {
        public Guid Id { get; set; }
        public long TelegramId { get; set; }
        public long ChatId { get; set; }
        public string Name { get; set; }
        public Guid? GroupId { get; set; }
        public virtual Group Group { get; set; }
    }
}
