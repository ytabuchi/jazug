using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using Newtonsoft.Json.Linq;
using Microsoft.WindowsAzure.MobileServices;

namespace AzureSample.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

			// Register for push notifications.
			var settings = UIUserNotificationSettings.GetSettingsForTypes(
				UIUserNotificationType.Alert
				| UIUserNotificationType.Badge
				| UIUserNotificationType.Sound,
				new NSSet());

			UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
			UIApplication.SharedApplication.RegisterForRemoteNotifications();

            LoadApplication(new AzureSample.App());

            return base.FinishedLaunching(app, options);
        }

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            const string templateBodyAPNS = "{\"aps\":{\"alert\":\"$(messageParam)\"}}";

            JObject templates = new JObject();
            templates["genericMessage"] = new JObject
            {
                { "body", templateBodyAPNS }
            };

            // Register for push with your mobile app
            Push push = TodoItemManager.DefaultManager.CurrentClient.GetPush();
            push.RegisterAsync(deviceToken, templates);
        }

        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            NSDictionary aps = userInfo.ObjectForKey(new NSString("aps")) as NSDictionary;

            string alert = string.Empty;
            if (aps.ContainsKey(new NSString("alert")))
                alert = (aps[new NSString("alert")] as NSString).ToString();

            //show alert
            if (!string.IsNullOrEmpty(alert))
            {
                UIAlertView avAlert = new UIAlertView("Notification", alert, null, "OK", null);
                avAlert.Show();
            }
        }
    }

    //public class Constants
    //{
    //    public static string ApplicationURL = @"https://xamarinmobileapps.azurewebsites.net";
    //    public static string NHUrl = @"https://XamarinMobileAppsNotificationHub.servicebus.windows.net";
    //    public static string NHAccessKey = @"YsVpAfP5pMa7rN/yaqdR8DSSzJg6jV9glLvcvczhJsk=";
    //    public static string NHHubName = @"XamarinMobileAppsNotificationHub";
    //}
}
