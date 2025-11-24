using Microsoft.Maui.Platform;

namespace lab3;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new AppShell();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = base.CreateWindow(activationState);

#if WINDOWS
        window.Created += (s, e) =>
        {
            var handle = WinRT.Interop.WindowNative.GetWindowHandle(window.Handler.PlatformView);
            var id = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(handle);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(id);

            appWindow.Closing += async (sender, args) =>
            {
                args.Cancel = true;

                bool answer = await Dispatcher.DispatchAsync(async () => 
                {
                    return await Current.MainPage.DisplayAlert("Вихід", "Ви точно хочете вийти?", "Так", "Ні");
                });

                if (answer)
                {
                    Current.Quit();
                }
            };
        };
#endif

        return window;
    }
}