using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Assets.SimpleAndroidNotifications;
using System;
using NotificationSamples;
using Lean.Localization;

public class PushNotification : MonoBehaviour
{
    private static PushNotification instance;
    public static PushNotification Instance { get { return instance; } }
    public const string ChannelId = "game_channel0";
    public const string ReminderChannelId = "reminder_channel1";
    public const string NewsChannelId = "news_channel2";
    private GameNotificationsManager manager;
    List<PushNotificationType> pushNotificationTypes;
    private bool hasInit = false;
    IEnumerator Start()
    {
        if (instance == null)
        {
            instance = this;
            manager = GetComponent<GameNotificationsManager>();
            var c1 = new GameNotificationChannel(ChannelId, "Default Game Channel", "Generic notifications", GameNotificationChannel.NotificationStyle.Popup, true, false, true, false, GameNotificationChannel.PrivacyMode.Public, new long[] { 0, 100 });
            var c2 = new GameNotificationChannel(NewsChannelId, "News Channel", "News feed notifications", GameNotificationChannel.NotificationStyle.Popup, true, false, true, false, GameNotificationChannel.PrivacyMode.Public, new long[] { 0, 100 });
            var c3 = new GameNotificationChannel(ReminderChannelId, "Reminder Channel", "Reminder notifications", GameNotificationChannel.NotificationStyle.Popup, true, false, true, false, GameNotificationChannel.PrivacyMode.Public, new long[] { 0, 100 });
            manager.Initialize(c1, c2, c3);
            manager.LocalNotificationDelivered += OnDelivered;
            manager.LocalNotificationExpired += OnExpired;
            yield return new WaitForSecondsRealtime(1);
            var latestNotification = manager.GetLastNotification();
            if (latestNotification != null)
            {
                Firebase.Analytics.FirebaseAnalytics.LogEvent("track_push_local", "type_push", latestNotification.Data);
            }
        }

    }
    public void SetUpNotification()
    {
        if (hasInit) return;
        hasInit = true;
        manager.CancelAllNotifications();
        SetUpAndroidNotification();
    }
    private void OnDestroy()
    {
        if (manager != null)
        {
            manager.LocalNotificationDelivered -= OnDelivered;
            manager.LocalNotificationExpired -= OnExpired;
        }
    }
    public void CancelNotification(int notificationId)
    {
        manager.CancelNotification(notificationId);
    }
    public void SendCompleteUpgradeNotification(int upgradeTime)
    {
        PushNotification.Instance.CancelNotification(11);
        DateTime pushtime = DateTime.Now.AddSeconds(upgradeTime);
        SendNotification(PushNotificationType.Upgrade, pushtime, PushNotificationType.Upgrade.ToString(), null, false, (int)PushNotificationType.Upgrade, ChannelId, "icon_small", "icon_big");
    }
    private void OnDelivered(PendingNotification deliveredNotification)
    {
    }
    private void OnExpired(PendingNotification obj)
    {
    }
    void SetUpAndroidNotification()
    {
        pushNotificationTypes = new List<PushNotificationType>() { PushNotificationType. package, PushNotificationType.Energ, PushNotificationType.IceCream,
                                                                   PushNotificationType.DailyMorning, PushNotificationType.DailyNight, PushNotificationType.DailyLogin,PushNotificationType.Upgrade};
        SetPush(PushNotificationType.DailyMorning, 24 + 8, 8, 8);
        SetPush(PushNotificationType.DailyLogin, 24 + 12, 12, 12);
        SetPush(PushNotificationType.DailyNight, 24 + 19, 19, 19);
        SetPush(PushNotificationType.Next12hour, 24 + 8 + 24, 8 + 24, 8);
        SetPush(PushNotificationType.Next24Hour, 24 + 12 + 48, 12 + 48, 12);
        SetPush(PushNotificationType.Next48hour, 24 + 8 + 72, 8 + 72, 8);
        if (DataController.Instance.Energy < 3)
        {
            int maxEn = 5 - DataController.Instance.Energy;
            int deltaTime = maxEn * 30 * 60;
            DateTime time = DateTime.Now.AddSeconds(deltaTime);
            //SetPushNotification(deltaTime, countID, LocalNotificationType.Normal, PushNotificationType.Energ);
            SendNotification(PushNotificationType.IceCream, time, PushNotificationType.IceCream.ToString(), null, false, (int)(PushNotificationType.IceCream), ChannelId, "icon_small", "icon_big");
        }
        if (DataController.Instance.IsItemUnlocked((int)ItemType.IceCream))
        {
            int deltaTime = DataController.Instance.IceCreamDuration;
            DateTime time = DateTime.Now.AddSeconds(deltaTime);
            if (deltaTime > 5 * 60)
            {
                SendNotification(PushNotificationType.IceCream, time, PushNotificationType.IceCream.ToString(), null, false, (int)(PushNotificationType.IceCream), ChannelId, "icon_small", "icon_big");
            }
        }
    }

    public void SetPush(PushNotificationType pDefault, int hour1, int hour2, int pushHour)
    {
        var time = DateTime.Now.Date.AddHours((DateTime.Now.Hour > pushHour) ? hour1 : hour2);
        if (!AddPush(time))
        {
            //SetPushNotification((int)diff.TotalSeconds, countID, LocalNotificationType.Normal, pDefault);
            SendNotification(pDefault, time, pDefault.ToString(), null, false, (int)(pDefault), ChannelId, "icon_small", "icon_big");
            //pushNotificationTypes.Remove(pDefault);
        }
    }

    bool AddPush(DateTime pushtime)
    {
        int deltaTime = 0;
        for (int i = 0; i < pushNotificationTypes.Count; i++)
        {
            switch (pushNotificationTypes[i])
            {
                case PushNotificationType.None:
                    break;
                case PushNotificationType.package:
                    double nowTimeStamp = DataController.ConvertToUnixTime(System.DateTime.Now);
                    float starterPackTimeStamp = PlayerPrefs.GetFloat(StarterPackController.STARTER_PACK_HASH, 0);
                    bool canDisplayStarterPack = DataController.Instance.GetLevelState(1, 7) > 0
                    && (nowTimeStamp - starterPackTimeStamp < 259200)
                    && PlayerPrefs.GetInt("has_buy_starter_pack", 0) == 0;
                    if (canDisplayStarterPack)
                    {
                        deltaTime = (int)(259200 - nowTimeStamp + starterPackTimeStamp);

                        if (Math.Abs(nowTimeStamp + deltaTime - DataController.ConvertToUnixTime(pushtime)) < 30 * 60)
                        {

                            //SetPushNotification(deltaTime, countID, LocalNotificationType.Normal, pushNotificationTypes[i]);
                            SendNotification(PushNotificationType.IceCream, pushtime, PushNotificationType.package.ToString(), null, false, (int)PushNotificationType.package, ChannelId, "icon_small", "icon_big");
                            pushNotificationTypes.Remove(pushNotificationTypes[i]);
                            return true;
                        }
                    }
                    else
                    {

                    }
                    break;
                case PushNotificationType.IceCream:
                    deltaTime = DataController.Instance.IceCreamDuration;
                    bool canDisplayIceCreamTruck = DataController.Instance.IsItemUnlocked((int)ItemType.IceCream);
                    DateTime tmptime = DateTime.Now.AddSeconds(deltaTime);
                    if (canDisplayIceCreamTruck && tmptime <= pushtime)
                    {
                        //SetPushNotification(deltaTime, countID, LocalNotificationType.Normal, pushNotificationTypes[i]);

                        SendNotification(PushNotificationType.IceCream, pushtime, PushNotificationType.IceCream.ToString(), null, false, (int)PushNotificationType.IceCream, ChannelId, "icon_small", "icon_big");
                        pushNotificationTypes.Remove(pushNotificationTypes[i]);
                        return true;
                    }
                    break;
            }
        }
        return false;
    }
    public void SendNotification(PushNotificationType notificationType, DateTime deliveryTime, string data, int? badgeNumber = null,
        bool reschedule = false, int id = 10, string channelId = null,
        string smallIcon = null, string largeIcon = null)
    {
        IGameNotification notification = manager.CreateNotification();
        if (notification == null)
            return;
        notification.Title = GetNotificationTitle(notificationType);
        notification.Body = GetNotificationDescription(notificationType);
        notification.Group = !string.IsNullOrEmpty(channelId) ? channelId : ChannelId;
        notification.DeliveryTime = deliveryTime;
        notification.Data = data;
        notification.SmallIcon = smallIcon;
        notification.LargeIcon = largeIcon;
        if (badgeNumber != null)
            notification.BadgeNumber = badgeNumber;
        notification.Id = id;
        PendingNotification notificationToDisplay = manager.ScheduleNotification(notification);
        notificationToDisplay.Reschedule = reschedule;
    }
    public string GetNotificationTitle(PushNotificationType pushNotificationType)
    {
        string title = "Notification";
        if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday && (int)pushNotificationType == 4)
        {
            title = Lean.Localization.LeanLocalization.GetTranslationText("PushNotificationType_SaturdayMorning_title");
        }
        else if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday && (int)pushNotificationType == 4)
        {
            title = Lean.Localization.LeanLocalization.GetTranslationText("PushNotificationType_SundayMorning_title");
        }
        else
        {
            title = Lean.Localization.LeanLocalization.GetTranslationText("PushNotificationType_" + pushNotificationType.ToString() + "_title");
        }
        return title;
    }
    public string GetNotificationDescription(PushNotificationType pushNotificationType)
    {
        string descreption = "Welcome to Cooking Love";
        if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday && (int)pushNotificationType == 4)
        {
            descreption = Lean.Localization.LeanLocalization.GetTranslationText("PushNotificationType_SaturdayMorning_des");
        }
        else if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday && (int)pushNotificationType == 4)
        {
            descreption = Lean.Localization.LeanLocalization.GetTranslationText("PushNotificationType_SundayMorning_des");
        }
        else
        {
            descreption = Lean.Localization.LeanLocalization.GetTranslationText("PushNotificationType_" + pushNotificationType.ToString() + "_des");
        }
        return descreption;
    }

}
public enum PushNotificationType
{
    None,
    package,
    Energ,
    IceCream,
    DailyMorning,
    DailyNight,
    DailyLogin,
    Next12hour,
    Next24Hour,
    Next72hour,
    Next48hour,
    Upgrade,
}

public enum LocalNotificationType
{
    None,
    Simple,
    Normal,
    Custom,
}