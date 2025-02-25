using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserControl_Library;
using UserControl_Library.NotificationUC.Enums;

namespace EasySave2._0.Models.Notifications_Related
{
	public class NotificationHelper
	{
		public static event EventHandler<Notification_UC>? NotificationAdded;
		//private static NotificationHelper? _instance;

		//#region Singleton
		//private NotificationHelper() { }

		//public static NotificationHelper? GetNotificationHelperInstance()
		//{
		//	return _instance ??= new NotificationHelper();
		//}

		//#endregion

		public static void CreateNotifcation(string title, string content, int type = 2)
		{
			App.Current.Dispatcher.Invoke(() =>
			{
				Notification_UC notification = new Notification_UC()
				{
					NotificationTitle = title,
					ContentText = content,
					NotificationType = (NotificationType)type
				};

				NotificationAdded?.Invoke(null, notification);
			});
		}
	}
}
