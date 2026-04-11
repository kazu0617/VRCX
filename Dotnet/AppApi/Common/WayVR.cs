using System;
using System.Diagnostics;
using System.IO;

namespace VRCX
{
    public partial class AppApi
    {
        /// <summary>
        /// Displays a notification via notify-send for Linux VR environments (WayVR, etc.).
        /// notify-send uses the freedesktop.org Desktop Notifications Specification (D-Bus),
        /// which WayVR and other Linux VR dashboards can display in VR.
        /// </summary>
        /// <param name="title">The title of the notification.</param>
        /// <param name="content">The content of the notification.</param>
        /// <param name="timeout">The duration of the notification in seconds.</param>
        /// <param name="image">The optional image file path to display in the notification.</param>
        public void WayVRNotification(string title, string content, int timeout, string image = "")
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = "notify-send",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                if (!string.IsNullOrEmpty(image) && File.Exists(image))
                {
                    startInfo.ArgumentList.Add("-i");
                    startInfo.ArgumentList.Add(image);
                }

                startInfo.ArgumentList.Add("-t");
                startInfo.ArgumentList.Add((timeout * 1000).ToString());
                startInfo.ArgumentList.Add("-a");
                startInfo.ArgumentList.Add("VRCX");
                startInfo.ArgumentList.Add("--");
                startInfo.ArgumentList.Add(title);
                startInfo.ArgumentList.Add(content);

                Process.Start(startInfo)?.Dispose();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Failed to send WayVR notification via notify-send");
            }
        }
    }
}
