using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using AndroidX.Core.View;
using CommunityToolkit.Mvvm.Messaging;

namespace CVJoyMAUI
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            WindowCompat.SetDecorFitsSystemWindows(this.Window, false);
            WindowInsetsControllerCompat windowInsetsController = new WindowInsetsControllerCompat(this.Window, this.Window.DecorView);
            // Hide system bars
            windowInsetsController.Hide(WindowInsetsCompat.Type.SystemBars());
            windowInsetsController.SystemBarsBehavior = WindowInsetsControllerCompat.BehaviorShowTransientBarsBySwipe;

            WeakReferenceMessenger.Default.Register<FullScreenMessage>(this, (r, m) =>
            {
                //IWindowInsetsController wicController = Window.InsetsController;
                //Window.SetDecorFitsSystemWindows(false);
                //Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);
                //if (wicController != null)
                //{
                //    wicController.Hide(WindowInsets.Type.NavigationBars());
                //}
                WindowCompat.SetDecorFitsSystemWindows(this.Window, false);
                WindowInsetsControllerCompat windowInsetsController = new WindowInsetsControllerCompat(this.Window, this.Window.DecorView);
                // Hide system bars
                windowInsetsController.Hide(WindowInsetsCompat.Type.SystemBars());
                windowInsetsController.SystemBarsBehavior = WindowInsetsControllerCompat.BehaviorShowTransientBarsBySwipe;
            });

            //WeakReferenceMessenger.Default.Register<NormalScreenMessage>(this, (r, m) =>
            //{
            //    IWindowInsetsController wicController = Window.InsetsController;
            //    Window.SetDecorFitsSystemWindows(true);
            //    Window.ClearFlags(WindowManagerFlags.Fullscreen);
            //    if (wicController != null)
            //    {
            //        wicController.Show(WindowInsets.Type.NavigationBars());
            //    }
            //});

        }

    }


}
