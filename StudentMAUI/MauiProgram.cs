using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Radzen;
using StudentMAUI.Data;
using System.Reflection;

namespace StudentMAUI;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
        var a = Assembly.GetExecutingAssembly();
        using var stream = a.GetManifestResourceStream("StudentMAUI.appsettings.json");

        var config = new ConfigurationBuilder()
                    .AddJsonStream(stream)
                    .Build();


        builder.Configuration.AddConfiguration(config);
        builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		builder.Services.AddSingleton<WeatherForecastService>();
        builder.Services.AddScoped<DialogService>();
        builder.Services.AddScoped<NotificationService>();
        builder.Services.AddScoped<TooltipService>();
        builder.Services.AddScoped<ContextMenuService>();
        builder.Services.AddScoped<StudentMAUI.ConDataService>();
        //builder.Services.AddDbContext<StudentMAUI.Data.ConDataContext>(options =>
        //{
        //    options.UseSqlServer(builder.Configuration.GetConnectionString("ConDataConnection"));
        //});
        builder.Services.AddDbContext<StudentMAUI.Data.ConDataContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetRequiredSection("ConnectionStrings").Get<ConnectionStrings>().ConDataConnection);
        });

        return builder.Build();
	}
}
