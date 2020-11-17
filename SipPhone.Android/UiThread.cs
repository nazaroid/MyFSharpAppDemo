using System;
using Android.OS;

namespace SipPhone.Android
{
    static class UiThread
    {
        private static Handler mHandler = new Handler(Looper.MainLooper);

        public static void Dispatch(Action action)
        {
            mHandler.Post(action);
        }
    }
}