using System.Linq;

namespace Domain
{
	public class RequestRecognizer
	{
		private readonly (RequestType Type, string Message)[] _requestTypeMessages = new (RequestType, string)[]
		{
			new (RequestType.Startup, "/start"),
			new (RequestType.Backward, "Назад."),
			new (RequestType.CreateGroup, "Создать расписание для группы."),
			new (RequestType.JoinGroup, "Присоединиться к группе."),
			new (RequestType.LeaveGroup, "Покинуть группу."),
			new (RequestType.WatchFullSchedule, "Всё расписание."),
			new (RequestType.WatchScheduleOnTomorrow, "Расписание на завтра."),
			new (RequestType.WatchScheduleOnToday, "Расписание на сегодня.")
		};

		public bool TryGetRequestType(string message, out RequestType request)
		{
			request = RequestType.None;

			var requestTypeMessage = _requestTypeMessages.FirstOrDefault(x => x.Message == message);

			if (requestTypeMessage == default)
				return false;

			request = requestTypeMessage.Type;
			return true;
		}
		public string GetMessage(RequestType request)
		{
			return _requestTypeMessages.First(x => x.Type == request).Message;
		}
	}
}
