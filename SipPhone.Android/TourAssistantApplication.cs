using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using SipPhone.Core.Models;

namespace SipPhone.Android
{
    [Application]
    public class TourAssistantApplication : Application, IBackgroundServiceCommandListener
    {
        public TourAssistantApplication(IntPtr p, JniHandleOwnership q) : base(p, q)
        { }

        public override void OnCreate()
        {
            base.OnCreate();

            log("OnCreate begin");

            AppDomain.CurrentDomain.UnhandledException += HandleExceptions;

            BackgroundServiceInputCommands.Listen(this);

            log("OnCreate end");
        }

        private void HandleExceptions(object sender, UnhandledExceptionEventArgs e)
        {
            SipPhoneLogger.Error<TourAssistantApplication>(e.ToString());
        }

        public void OnStartCmd()
        {
            log("OnStartCmd begin");

            StartService(new Intent(this, typeof(SipPhoneBackgroundService)));

            log("OnStartCmd end");
        }

        private static void log (string msg)
        {
            SipPhoneLogger.Warn<TourAssistantApplication>(msg);
        }

        public void OnRaiseRegistrationStatusCmd()
        { 
            var status = LinphoneCoreAccessor.Get().DefaultProxyConfig.State.ToRegistrationStatus();
            BackgroundServiceNotifications.NotifyRegistrationStatus(status);
        }
    }
}