using System;
using System.Collections.Generic;
using SipPhone.Core.Models;

namespace SipPhone.Android
{
    internal class TraceNotifier : ITraceNotifier
    {
        private static readonly List<ITraceListener> _listeners = new List<ITraceListener>();

        public void RegisterListener(ITraceListener listener)
        {
            _listeners.Add(listener);
        }

        public static readonly List<ITraceListener> Listeners = new List<ITraceListener>();

        public static void Notify(string msg)
        {
            SipPhoneLogger.Warn<TraceNotifier>(string.Format("Notify: msg='{0}'", msg));
            _listeners.ForEach(x => x.OnTrace(msg));
        }
    }
}