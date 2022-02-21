using DosetteReminder.TelemetryStorageClient;
using DosetteReminder.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace DosetteReminder;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});
		builder.Services.AddHttpClient();
		builder.Services.AddSingleton<ITelemetryStorageClient, TtnTelemetryStorageClient>();
		builder.Services.AddSingleton<MainPage>();
		builder.Services.AddSingleton<ReminderMainViewModel>();

		return builder.Build();
	}
}
