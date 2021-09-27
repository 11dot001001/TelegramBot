using System;
using System.Linq;

namespace Domain
{
	public static class RequestService
	{
		private class RequestMessage
		{
			public RequestType Request { get; set; }
			public string Message { get; set; }

			public RequestMessage(RequestType request, string message)
			{
				Request = request;
				Message = message ?? throw new ArgumentNullException(nameof(message));
			}
		}

		static private readonly RequestMessage[] _requestMessages = new RequestMessage[]
		{
			new RequestMessage(RequestType.Startup, "/start"),
			new RequestMessage(RequestType.Backward, "Назад."),
			new RequestMessage(RequestType.CreateGroup, "Создать расписание для группы."),
			new RequestMessage(RequestType.JoinGroup, "Присоединиться к группе."),
			new RequestMessage(RequestType.LeaveGroup, "Покинуть группу."),
			new RequestMessage(RequestType.WatchFullSchedule, "Всё расписание."),
			new RequestMessage(RequestType.WatchScheduleOnTomorrow, "Расписание на завтра."),
			new RequestMessage(RequestType.WatchScheduleOnToday, "Расписание на сегодня."),
		};

		static public bool TryGetRequestByMessage(string message, out RequestType request)
		{
			request = RequestType.None;
			RequestMessage requestMessage = _requestMessages.FirstOrDefault(x => x.Message == message);

			if (requestMessage == default)
				return false;

			request = requestMessage.Request;
			return true;
		}
		static public string GetMessageByRequest(RequestType request)
		{
			return _requestMessages.First(x => x.Request == request).Message;
		}
	}
}
