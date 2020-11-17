using Android.App;
using Android.Content.PM;
using Android.OS;
using SipPhone.Android;
using TourAssistant.Client.Core.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace TourAssistant.Client.Android
{
    [Activity(
        Label = "Tour Assistant",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : FormsApplicationActivity
    {
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

            SipPhoneLogger.Warn<MainActivity>("OnCreate begin");

            Forms.Init (this, bundle);

            LoadApplication(new App(new Bootstrapper()));

            SipPhoneLogger.Warn<MainActivity>("OnCreate end");
        }
    }
}

