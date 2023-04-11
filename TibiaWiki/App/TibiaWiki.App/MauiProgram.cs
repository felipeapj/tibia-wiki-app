using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using TibiaWiki.WebScrapper;

namespace TibiaWiki.App
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<DataLoader>(); 
            builder.Services.AddScoped<MainPage>(); 
            builder.Services.AddScoped<HttpClient>(); 
            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}