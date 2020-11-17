using System.Threading.Tasks;
using Org.Linphone.Core;
using SipPhone.Core.Models;
using System.Threading;
using Android.Runtime;
using Java.Interop;

namespace SipPhone.Android
{
    public class OutcomingCall : IOutcomingCall
    {
        private OutcommingCallItem activeCall;

        public void Begin(string destanationSipAddress)
        {
            if(activeCall != null) log("call already in progress");

            log("begin");

            Task.Run(() => activeCall = MakeCall(destanationSipAddress));
        }

        private OutcommingCallItem MakeCall(string destanationSipAddress)
        {
            var newCall = new OutcommingCallItem(this);
            if(newCall.Start(destanationSipAddress))
            {
                return newCall;
            }

            return activeCall;
        }

        internal void OnTerminated(OutcommingCallItem call)
        {
            activeCall = null;
        }

        public void End()
        {
            Task.Run(() => activeCall.End());
        }

        private void log(string msg)
        {
            SipPhoneLogger.Warn<OutcomingCall>(msg);
        }
    }

    public class OutcommingCallItem
    {
        private ILinphoneCall call;
        private EndCallListener endCallListener;
        OutcomingCall owner;

        public OutcommingCallItem(OutcomingCall owner)
        {
            this.owner = owner;
        }

        public bool Start(string destanationSipAddress)
        {
            endCallListener = new EndCallListener(this);
            LinphoneCoreAccessor.AddListener(endCallListener);

            try
            {
                if (StartInternal(destanationSipAddress))
                {
                    return true;
                }
            }
            catch (Java.Lang.Exception e)
            {
                SipPhoneLogger.Error<OutcomingCall>(e.Message);
            }

            LinphoneCoreAccessor.Get().RemoveListener(endCallListener);
            return false;
        }

        private bool StartInternal(string destanationSipAddress)
        {
            const bool forbid_self_call = true;

            bool started = false;

            notify("Begin calling to " + destanationSipAddress);

            log("Notify registers");
            LinphoneCoreAccessor.Get().RefreshRegisters();
            log("registers notified");

            ILinphoneAddress lAddress;
            try
            {
                log("address parsing");
                lAddress = LinphoneCoreAccessor.Get().InterpretUrl(destanationSipAddress);
                log("address parsed");
                var lpc = LinphoneCoreAccessor.Get().DefaultProxyConfig;

                if (forbid_self_call && lpc != null && lAddress.AsStringUriOnly().Equals(lpc.Identity))
                {
                    log("self call canceled");
                    return false;
                }
            }
            catch (LinphoneCoreException e)
            {
                log(e.Message);
                return false;
            }
            lAddress.DisplayName = destanationSipAddress;

            // Send the INVITE message to destination SIP address
            log("begin invite address: " + lAddress.AsString());
            call = LinphoneCoreAccessor.Get().Invite(lAddress);
            if (call == null)
            {
                notify("Could not place call to " + destanationSipAddress);
                notify("Aborting");
                return false;
            }
            started = true;
            notify("Call to " + destanationSipAddress + " is in progress...");

            return started;
        }

        public void End()
        {
            log("begin End");
            log("Remove endCallListener");
            LinphoneCoreAccessor.Get().RemoveListener(endCallListener);
            if (LinphoneCoreAccessor.Get().IsIncall)
            {
                TraceNotifier.Notify("OutcomingCall: terminating...");
                LinphoneCoreAccessor.Get().TerminateCall(call);
                call.Dispose();
            }
            log("owner.OnTerminated");
            owner.OnTerminated(this);
            owner = null;
            log("finish End");
            TraceNotifier.Notify("OutcomingCall: ended");
        }

        private void log(string msg)
        {
            SipPhoneLogger.Warn<OutcomingCall>(msg);
        }

        private void notify(string msg)
        {
            TraceNotifier.Notify(string.Format("OutcomingCall: {0}", msg));

        }
    }

    public class EndCallListener : LinphoneCoreListenerBase
    {
        private OutcommingCallItem _call;
        public EndCallListener(OutcommingCallItem call)
        {
            _call = call;
        }

        public override void RegistrationState(ILinphoneCore p0, ILinphoneProxyConfig p1, LinphoneCoreRegistrationState crstate, string msg)
        {
            base.RegistrationState(p0, p1, crstate, msg);

            log("Registration state: " + msg);
        }

        public override void CallState(ILinphoneCore lc, ILinphoneCall p1, LinphoneCallState cstate, string msg)
        {
            log(string.Format("CallState: cstate='{0}'; msg='{1}'", cstate, msg));

            if (LinphoneCallState.CallEnd.Equals(cstate))
            {
                log("CallEnd state");
                _call.End();
            }
        }

        private void log(string msg)
        {
            SipPhoneLogger.Warn<EndCallListener>(msg);
        }
    }
}