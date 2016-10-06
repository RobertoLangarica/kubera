///----------------------------------------------
/// Flurry Analytics Plugin
/// Copyright © 2016 Aleksei Kuzin
///----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KHD {

    public class FlurryAnalyticsTest : MonoBehaviour {

        public bool startSessionFromCode = true;

    	private void Start() {
            if (!startSessionFromCode) {
                return;
            }

            KHD.FlurryAnalytics.Instance.SetDebugLogEnabled(true);

            // Enable data replication to UnityAnalytics.
            // KHD.FlurryAnalytics.Instance.replicateDataToUnityAnalytics = true;
            
            // Set custom app version.
            // KHD.FlurryAnalytics.Instance.SetAppVersion("1.0.0");

            // Track unique user info.
            // KHD.FlurryAnalytics.Instance.SetUserId("ioufewjkhjwdfsmnfbweoiufj"/* replace with user unique id */);
            // KHD.FlurryAnalytics.Instance.SetGender(KHD.FlurryAnalytics.Gender.Male);
            // KHD.FlurryAnalytics.Instance.SetAge(32);

            // Just one line of code to get wide range of metrics like:
            // Sessions
            // Active Users
            // New Users
            // Session Length
            // Frequency of Use
            // Custom User Segments
            // User Retention
            // Version Adoption
            // Devices
            // Carriers
            // Firmware Versions
            KHD.FlurryAnalytics.Instance.StartSession(
                "DHQ7W44SGF7D4N7R672C", // - replace with your API KEY.
                "ZPP8WWPDGBJR49CF2VTC", // - replace with your API KEY.
                true);
        }

        private void OnGUI() {
            var index = 0;

            if (Button("Log Event", index++)) {
                KHD.FlurryAnalytics.Instance.LogEvent("KHD Sample Event");
            }
            if (Button("Log Event Wit Parameters", index++)) {
                KHD.FlurryAnalytics.Instance.LogEventWithParameters(
                    "KHD Sample Event with parameters",
                    new Dictionary<string, string>() {
                        { "Param1", "Value1" },
                        { "Param2", "Value2" },
                        { "Param3", "Value3" },
                });
            }
            if (Button("Log Timed Event", index++)) {
                KHD.FlurryAnalytics.Instance.LogEvent("KHD Sample Timed Event", true);
            }
            if (Button("End Timed Event", index++)) {
                KHD.FlurryAnalytics.Instance.EndTimedEvent("KHD Sample Timed Event");
            }
        }

        private bool Button(string label, int index) {
            var width = Screen.width * 0.8f;
            var height = Screen.height * 0.07f;

            var rect = new Rect((Screen.width - width) * 0.5f,
                                index * height + height * 0.5f,
                                width, height);
            return GUI.Button(rect, label);
        }
    }

}
