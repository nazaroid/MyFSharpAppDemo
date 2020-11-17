using System;
using Android.Util;

namespace SipPhone.Android
{
    public static class SipPhoneLogger
    {
        private const string Tag = "Sip-phone";

        public static void Info<T>(string msg)
        {
            Info(typeof(T), msg);
        }

        public static void Debug<T>(string msg)
        {
            Debug(typeof(T), msg);
        }

        public static void Verbose<T>(string msg)
        {
            Verbose(typeof(T), msg);
        }

        public static void Warn<T>(string msg)
        {
            Warn(typeof(T), msg);
        }

        public static void Error<T>(string msg)
        {
            Error(typeof(T), msg);
        }


        public static void Info(Type source, string msg)
        {
            Log.Info(Tag, FormatMsg(source, msg));
        }

        public static void Debug(Type source, string msg)
        {
            Log.Debug(Tag, FormatMsg(source, msg));
        }

        public static void Verbose(Type source, string msg)
        {
            Log.Verbose(Tag, FormatMsg(source, msg));
        }

        public static void Warn(Type source, string msg)
        {
            Log.Warn(Tag, FormatMsg(source, msg));
        }
        public static void Error(Type source, string msg)
        {
            Log.Warn(Tag, FormatMsg(source, msg));
        }

        private static string FormatMsg(Type source, string msg)
        {
            return string.Format("{0}: {1}", source.Name, msg);
        }
    }
}