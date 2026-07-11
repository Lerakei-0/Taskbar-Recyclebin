using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace RecycleBinTaskbar
{
    /// <summary>
    /// Shows a simple toast notification. Uses the AppUserModelID that was already
    /// set via SetCurrentProcessExplicitAppUserModelID and the desktop shortcut
    /// that carries the same ID - no extra registration is required.
    /// </summary>
    internal static class ToastHelper
    {
        public static void ShowSimpleToast(string appId, string title, string message)
        {
            try
            {
                string xml = $@"
                    <toast>
                        <visual>
                            <binding template=""ToastGeneric"">
                                <text>{title}</text>
                                <text>{message}</text>
                            </binding>
                        </visual>
                    </toast>";

                var doc = new XmlDocument();
                doc.LoadXml(xml);

                var toast = new ToastNotification(doc);
                ToastNotificationManager.CreateToastNotifier(appId).Show(toast);
            }
            catch
            {
                // Toasts are a nice-to-have, not critical - if the platform or the
                // user's notification settings don't support them, fail silently
                // rather than interrupting the Empty Recycle Bin action.
            }
        }
    }
}
