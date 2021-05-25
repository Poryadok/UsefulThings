using PM.UsefulThings;
using PM.UsefulThings.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.UsefulThings.Vibration
{
    public static class Vibrator
    {
#if UNITY_ANDROID
        static AndroidJavaObject vibrator = null;
#endif
        static Vibrator()
        {
#if UNITY_ANDROID
            var unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var unityPlayerActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
            vibrator = unityPlayerActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
#endif
        }
        public static void Vibrate(long time)
        {
#if UNITY_ANDROID
            if (vibrator != null)
                vibrator.Call("vibrate", time);
#endif
        }
    }
}