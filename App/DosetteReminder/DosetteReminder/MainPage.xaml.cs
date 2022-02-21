using DosetteReminder.ViewModels;
using System.Diagnostics;
using System.Web;

namespace DosetteReminder;

public partial class MainPage : ContentPage
{
	public MainPage(ReminderMainViewModel reminderMainView)
	{
		InitializeComponent();
		this.BindingContext = reminderMainView;
	}
}

