using SipPhone.Core.Models;

namespace SipPhone.Android
{
    class DroidSipPhonePlatform : ISipPhonePlatform
    {
        public IOutcomingCall OutcomingCall()
        {
            return new OutcomingCall();
        }

        public ITraceNotifier TraceNotifier()
        {
            return new TraceNotifier();
        }
    }
}