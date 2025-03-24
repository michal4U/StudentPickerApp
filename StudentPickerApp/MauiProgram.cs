using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
#if WINDOWS
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Windows.Graphics;
#endif

namespace StudentPickerApp;

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
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });


#if WINDOWS
        builder.ConfigureLifecycleEvents(events =>
        {
            events.AddWindows(windowLifecycleBuilder =>
            {
                windowLifecycleBuilder.OnWindowCreated(window =>
                {
                    IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
                    var appWindow = AppWindow.GetFromWindowId(Win32Interop.GetWindowIdFromWindow(hWnd));

                    if (appWindow is not null)
                    {
                        Application.Current.MainPage.Title = "💀💀Maszyna Tortur💀💀";

                        appWindow.Resize(new SizeInt32(650, 950)); // Szerokość i wysokość okna
                        appWindow.Move(new PointInt32(100, 100)); // Pozycja na ekranie
                    }
                });
            });
        });
#endif

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
