using System;
using Java.Lang;
using Org.Linphone.Core;
using Android.Content;
using Android.OS;

namespace SipPhone.Android
{
    static class LinphoneCoreAccessor
    {
        private static ILinphoneCore _lc;
        const bool isDebugLogEnabled = false;

        public static void AddListener(ILinphoneCoreListener listener)
        {
            log("adding listener for 'Linphone core'");

            if (listener == null) throw new ArgumentNullException(nameof(listener));

            _lc.AddListener(listener);

            log("listener added");
        }

        public static void Create(Context c, ILinphoneCoreListener listener)
        {
            log("Create begin");

            if (c == null) throw new ArgumentNullException(nameof(c));
            if (listener == null) throw new ArgumentNullException(nameof(listener));

            EnableLogging(c);

            log("creating core");
            _lc = LinphoneCoreFactory.Instance().CreateLinphoneCore(listener, null, null, null, c);
            log("core created");

            AddAccount(_lc);
            log("account created");

            log("Create end");
        }

        private static void EnableLogging(Context c)
        {
            var logPath = c.FilesDir.AbsolutePath;
            log("enable core logging to: " + logPath);

            //Enable verbose traces
            LinphoneCoreFactory.Instance().SetDebugMode(isDebugLogEnabled, "Linphone");

            ////Enable the linphone core log collection to upload logs on a server.
            //LinphoneCoreFactory.Instance().EnableLogCollection(isDebugLogEnabled);
            ////Set the path where the log files will be written for log collection.
            //LinphoneCoreFactory.Instance().SetLogCollectionPath(logPath);

            LinphoneCoreFactory.Instance().SetLogHandler(new LinphoneCoreLogHandler());
        }

        public static ILinphoneCore Get()
        {
            return _lc;
        }

        private static void AddAccount(ILinphoneCore lc)
        {
            string tempUsername = "test1";
            string tempUserId = "test1";
            string tempDisplayName = "test1";
            string tempPassword = "1234";
            //string tempDomain = "192.168.1.21"; //HOME
            //string tempProxy = "192.168.1.21";
            string tempDomain = "172.16.4.236"; // JOB
            string tempProxy = "172.16.4.236";
            string tempRealm = null;

            var tempNoDefault = false;
            var tempTransport = LinphoneAddressTransportType.LinphoneTransportUdp;
            var tempOutboundProxy = false;
            var tempEnabled = true;
            string tempContactsParams = null;
            int tempExpire = 5000;
            bool tempAvpfEnabled = false;
            int tempAvpfRRInterval = 0;
            var tempQualityReportingEnabled = false;
            string tempQualityReportingCollector = null;
            int tempQualityReportingInterval = 0;

            if (tempUsername == null || tempUsername.Length < 1 || tempDomain == null || tempDomain.Length < 1)
            {
                throw new ArgumentException("Skipping account save: username or domain not provided");
            }

            string identity = "sip:" + tempUsername + "@" + tempDomain;
            string proxy = "sip:";
            if (tempProxy == null)
            {
                proxy += tempDomain;
            }
            else
            {
                if (!tempProxy.StartsWith("sip:") && !tempProxy.StartsWith("<sip:")
                    && !tempProxy.StartsWith("sips:") && !tempProxy.StartsWith("<sips:"))
                {
                    proxy += tempProxy;
                }
                else
                {
                    proxy = tempProxy;
                }
            }
            var proxyAddr = LinphoneCoreFactory.Instance().CreateLinphoneAddress(proxy);
            var identityAddr = LinphoneCoreFactory.Instance().CreateLinphoneAddress(identity);

            if (tempDisplayName != null)
            {
                identityAddr.DisplayName = tempDisplayName;
            }

            if (tempTransport != null)
            {
                proxyAddr.Transport = tempTransport;
            }

            string route = tempOutboundProxy ? proxyAddr.AsStringUriOnly() : null;

            var prxCfg = lc.CreateProxyConfig(identityAddr.AsString(), proxyAddr.AsStringUriOnly(), route, tempEnabled);

            if (tempContactsParams != null)
                prxCfg.ContactUriParameters = tempContactsParams;
            if (tempExpire != null)
            {
                try
                {
                    prxCfg.Expires = tempExpire;
                }
                catch (NumberFormatException nfe) { }
            }

            prxCfg.EnableAvpf(tempAvpfEnabled);
            prxCfg.AvpfRRInterval = tempAvpfRRInterval;
            prxCfg.EnableQualityReporting(tempQualityReportingEnabled);
            prxCfg.QualityReportingCollector = tempQualityReportingCollector;
            prxCfg.QualityReportingInterval = tempQualityReportingInterval;

            if (tempRealm != null)
                prxCfg.Realm = tempRealm;

            var authInfo = LinphoneCoreFactory.Instance().CreateAuthInfo(tempUsername, tempUserId, tempPassword, null, null, tempDomain);

            lc.AddProxyConfig(prxCfg);
            lc.AddAuthInfo(authInfo);

            if (!tempNoDefault)
                lc.DefaultProxyConfig = prxCfg;
        }

        private static void log(string msg)
        {
            SipPhoneLogger.Warn(typeof(LinphoneCoreAccessor), msg);
        }

        internal sealed class LinphoneCoreLogHandler : Handler, ILinphoneLogHandler
        {
            public void Log(string loggerName, int level, string levelString, string msg, Throwable e)
            {
                SipPhoneLogger
                    .Verbose<LinphoneCoreLogHandler>(string.Format("loggerName = '{0}'; level = {1}, levelString = '{2}', msg = '{3}', Throwable = '{4}'"
                                          , loggerName, level, levelString, msg, e));
            }
        }
    }
}