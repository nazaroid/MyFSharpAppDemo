using System;
using Android.Util;
using TourAssistant.Client.Core.Platform;

namespace TourAssistant.Client.Android
{
    sealed class Logger : ILogger
    {
        public void Error(Exception e)
        {
            Log.Error("Tour assistant" , e.ToString());
        }

        public void Warn(string msg)
        {
            Log.Warn("Tour assistant", msg);
        }
    }
}