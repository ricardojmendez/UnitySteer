using System;
using UnityEngine;

#if UNITY_ANDROID

namespace Uniject.Impl {
    public class AndroidPlatformUtil : IPlatformUtil {
        private ILogger logger;
        private AndroidJavaObject androidUtilInstance;

        public AndroidPlatformUtil(ILogger logger) {
            this.logger = logger;
            logger.prefix = "AndroidPlatformUtil";

            // Get the activity context for Android.
            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
            AndroidJavaObject currentActivity = jc.GetStatic<AndroidJavaObject>("currentActivity");

            logger.Log("Got current activity");

            // Create java class object.
            AndroidJavaClass androidUtilClass = new AndroidJavaClass("com.ballatergames.util.AndroidUtil");
            androidUtilClass.CallStatic("initialise", currentActivity);
            androidUtilInstance = androidUtilClass.CallStatic<AndroidJavaObject>("getInstance");

            logger.Log("Got android util instance");
        }

        public void launchTwitter(string tweetText, string url) {
            logger.Log("Attempting twitter launch...");
            androidUtilInstance.Call("launchTwitter", tweetText, url);
        }

        public void launchReviewRequest() {
            Application.OpenURL("market://details?id=com.ballatergames.corpus");
        }
    }
}

#endif
