using System;
using Android.App;
using System.Threading.Tasks;
using Org.Linphone.Core;
using SipPhone.Core.Models;

namespace SipPhone.Android
{
    internal static class LinphoneCoreLoop
    {
        static Task iterateLoopTask;
        const int loopInterval = 200;

        public static void Run()
        {
            log("Run begin");

            log("initing core");
            var listener = new HandlingLinphoneCoreListener();
            LinphoneCoreAccessor.Create(Application.Context.ApplicationContext, listener);
            log("core inited");

            iterateLoopTask = Task.Run(() =>
            {
                while (true)
                {
                    UiThread.Dispatch(() =>
                    {
                        //log("Iteration begin");
                        try
                        {
                            LinphoneCoreAccessor.Get().Iterate();
                        }
                        catch (Exception e)
                        {
                            err(e.Message);
                        }
                        //log("Iteration end");
                    });
                    System.Threading.Thread.Sleep(loopInterval);
                }
            });

            log("Run end");
        }

        private static void log(string msg)
        {
            SipPhoneLogger.Warn(typeof(LinphoneCoreLoop), msg);
        }

        private static void err(string msg)
        {
            SipPhoneLogger.Error(typeof(LinphoneCoreLoop), msg);
        }
        
    }

    public class HandlingLinphoneCoreListener : LinphoneCoreListenerBase
    {
        public override void CallState(ILinphoneCore p0, ILinphoneCall p1, LinphoneCallState state, string msg)
        {
            //base.CallState(p0, p1, cstate, msg);

            log(string.Format("notify CallState: state = {0}; message = {1}", state, msg));
            TraceNotifier.Notify(string.Format(":: Call state: state = {0}; message = {1}", state, msg));

            if (LinphoneCallState.IncomingReceived.Equals(state))
            {
                log("incoming call");
                var incomingCall = new IncomingCall(p1);
                BackgroundServiceNotifications.NotifyIncomingCall(incomingCall);
            }
        }

        public override void RegistrationState(ILinphoneCore p0, ILinphoneProxyConfig p1, LinphoneCoreRegistrationState state, string msg)
        {
            //base.RegistrationState(p0, p1, p2, p3);
            log(string.Format("notify RegistrationState: state = {0}; message = {1}", state, msg));

            BackgroundServiceNotifications.NotifyRegistrationStatus(state.ToRegistrationStatus());
        }

        private void log(string msg)
        {
            SipPhoneLogger.Warn<HandlingLinphoneCoreListener>(msg);
        }
    }
}