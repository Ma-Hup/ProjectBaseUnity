using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants {

#region 示例

    public const string USER_INFO_URL = "eg";
    public enum EVENT_UNLOCK {
        DAILY_CHALLENGE,
        JOURNEY,
        DAILY_QUEST,
        MASTER_LEVEL,
        None
    }
    public class Event
    {
        public const string ORIENTATION_CHANGE = "OrientationChange";
        public const string SCREEN_SAFE_AREA_CHANGE = "ScreenSafeAreaChange";
        public const string EVENTS_TEST = "EventTest";
    }
#endregion



}
