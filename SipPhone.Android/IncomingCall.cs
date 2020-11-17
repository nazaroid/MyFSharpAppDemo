using Org.Linphone.Core;
using SipPhone.Core.Models;

namespace SipPhone.Android
{
    class IncomingCall : IIncomingCall
    {
        private ILinphoneCall _linphoneCall;

        public IncomingCall(ILinphoneCall linphoneCall)
        {
            _linphoneCall = linphoneCall;
        }

        public void End()
        {
            log("begin End");
            LinphoneCoreAccessor.Get().TerminateCall(_linphoneCall);
            _linphoneCall.Dispose();
            log("finish End");
        }

        private void log(string msg)
        {
            SipPhoneLogger.Warn<IncomingCall>(msg);
        }
    }
}