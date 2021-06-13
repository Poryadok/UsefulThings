using PM.UsefulThings;
using PM.UsefulThings.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.UsefulThings.Vibration
{
    public static class Vibrator
    {
        private static bool _isVibrationOn;
        public static bool IsVibrationOn
        {
            get => _isVibrationOn;
            set
            {
                if (_isVibrationOn != value)
                {
                    _isVibrationOn = value;
                    PlayerPrefs.SetInt("DisableVibration", value ? 0 : 1);
                }
            }
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        static AndroidJavaObject vibrator = null;
#endif
        static Vibrator()
        {
            _isVibrationOn = PlayerPrefs.GetInt("DisableVibration") == 0;
#if UNITY_ANDROID && !UNITY_EDITOR
            var unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var unityPlayerActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
            vibrator = unityPlayerActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
#endif
        }
        public static void Vibrate(long time)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (IsVibrationOn && vibrator != null)
                vibrator.Call("vibrate", time);
#endif
        }
    }
}