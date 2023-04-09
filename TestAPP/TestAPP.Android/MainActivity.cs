using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Android.Widget;
using Android.Views;
using Xamarin.Forms;
using MediaManager;

namespace TestAPP.Droid
{
    [Activity(Label = "Family", Icon = "@mipmap/Test", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]

    // Je crois n'avoir rien touché ici, tous s'est ajouté automatiquement
    // je t'aime zigouigoui
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            Xamarin.FormsMaps.Init(this, savedInstanceState);
            LoadApplication(new App());
            CrossMediaManager.Current.Init();

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                Window.SetStatusBarColor(Android.Graphics.Color.ParseColor("#000000"));
            }

            // ah si je me souviens d'avoir ajouté ça, c'etait pour regler le soucis de couleur entre nos telephone (spoiler; ça na pas marché)
            Xamarin.Forms.Application.Current.UserAppTheme = OSAppTheme.Dark;
        }

        //J'imagique que ça a un rapport avec toutes les permissions necessaire pour l'APP
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
                
    }
}

