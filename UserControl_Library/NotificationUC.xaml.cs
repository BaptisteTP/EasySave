using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UserControl_Library.NotificationUC.Enums;

namespace UserControl_Library
{
	public partial class Notification_UC : UserControl
	{
		public string NotificationTitle
		{
			get { return (string)GetValue(NotificationTitleProperty); }
			set { SetValue(NotificationTitleProperty, value); }
		}

		public static readonly DependencyProperty NotificationTitleProperty =
			DependencyProperty.Register("NotificationTitle", typeof(string), typeof(Notification_UC), new PropertyMetadata("Base Title"));

		public string ContentText
		{
			get { return (string)GetValue(ContentTextProperty); }
			set { SetValue(ContentTextProperty, value); }
		}

		public static readonly DependencyProperty ContentTextProperty =
			DependencyProperty.Register("ContentText", typeof(string), typeof(Notification_UC), new PropertyMetadata("Base Content"));

		public NotificationType NotificationType
		{
			get { return (NotificationType)GetValue(NotificationTypeProperty); }
			set { SetValue(NotificationTypeProperty, value); }
		}

		public static readonly DependencyProperty NotificationTypeProperty =
			DependencyProperty.Register("NotificationType", typeof(NotificationType), typeof(Notification_UC), new PropertyMetadata(null));

		public Notification_UC()
		{
			InitializeComponent();
		}

		public void StartAnimation()
		{
			NotificationUC.RenderTransform = new TranslateTransform(0, 150);

			Duration duration = new Duration(new TimeSpan(0, 0, 0, 0, 400));
			DoubleAnimation anim = new DoubleAnimation(0, duration);
			anim.AccelerationRatio = 0.8;

			NotificationUC.RenderTransform.BeginAnimation(TranslateTransform.YProperty, anim);
		}

		public void EndAnimation()
		{
			Duration duration = new Duration(new TimeSpan(0, 0, 0, 0, 400));
			DoubleAnimation anim = new DoubleAnimation(500, duration);
			anim.AccelerationRatio = 0.7;

			NotificationUC.RenderTransform.BeginAnimation(TranslateTransform.XProperty, anim);
		}

		private void NotificationUC_MouseDown(object sender, MouseButtonEventArgs e)
		{
			EndAnimation();
		}
    }

}
