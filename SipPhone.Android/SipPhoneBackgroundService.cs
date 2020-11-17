using Android.App;
using Android.Content;
using Android.OS;

namespace SipPhone.Android
{
    [Service(Label = "Sip-phone service")]
    public class SipPhoneBackgroundService : Service
    {
        public override void OnCreate()
        {
            base.OnCreate();

            log("OnCreate begin");

            LinphoneCoreLoop.Run();
            PreventDestroing();

            log("OnCreate end");
        }
        private void PreventDestroing()
        {
            log("StartForeground begin");
            var builder = new Notification.Builder(this)
                .SetSmallIcon(Resource.Drawable.Icon)
                .SetTicker("Sip-phone service was runned");
            var notification = builder.Build();
            StartForeground(0, notification);
            log("StartForeground end");
        }

        public override void OnDestroy()
        {
            log("OnDestroy");
            base.OnDestroy();
        }

        private void log(string msg)
        {
            SipPhoneLogger.Warn<SipPhoneBackgroundService>(msg);
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }
    }
}

   