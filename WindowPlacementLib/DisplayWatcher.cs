using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowPlacementLib.Models;

namespace WindowPlacementLib
{
    public class DisplayWatcher : IDisposable
    {
        protected SharedUtilties.Logger logger;
        protected int monitorCount;
        protected System.Timers.Timer saveWindowsTimer;
        private object lockObject = new object();
        protected Dictionary<int, MonitorConfiguration> monitorConfigurations = new Dictionary<int, MonitorConfiguration>();
        protected bool updatingWindows = false;

        public DisplayWatcher()
        {
            monitorCount = SystemInformation.MonitorCount;

            SystemEvents.DisplaySettingsChanged += new EventHandler(SystemEvents_DisplaySettingsChanged);

            //var allWindows = WindowHelpers.FindWindowsWithText("Slack");
            //var windowTitles = allWindows.Select(win => WindowHelpers.GetWindowText(win)).ToList();
            //var windowRects = allWindows.Select(win => WindowHelpers.GetWindowRect(win)).ToList();

            //allWindows.ForEach(hWnd => WindowHelpers.MoveWindowScreenLeft(hWnd));
            var desktopWindows = WindowHelpers.GetDesktopWindows();

            var interval = 4 * 60 * 1000;
#if DEBUG
            interval = 2 * 60 * 1000;
#endif
            saveWindowsTimer = new System.Timers.Timer(interval);
            saveWindowsTimer.AutoReset = true;
            saveWindowsTimer.Elapsed += SaveWindowsTimer_Elapsed;
            saveWindowsTimer.Start();

            var currentPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            logger = new SharedUtilties.Logger(Path.Combine(currentPath, "Log.log"));
            logger.WriteLine("Window watching initialized");
        }

        private void SaveWindowsTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                SaveWindows();
            }
            catch (Exception ex)
            {
                logger.WriteLine(ex.ToString());
            }
        }

        private void SaveWindows()
        {
            if (updatingWindows)
            {
                return;
            }

            lock (lockObject)
            {
                var desktopWindows = WindowHelpers.GetDesktopWindows();
                var monitorConfiguration = new MonitorConfiguration()
                {
                    MonitorCount = monitorCount,
                    Windows = desktopWindows
                };

                monitorConfigurations[monitorCount] = monitorConfiguration;
            }
        }

        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            var newMonitorCount = SystemInformation.MonitorCount;
            try
            {
                logger.WriteLine($"Display changed detected - monitor count: {newMonitorCount} (previous: {this.monitorCount}");

                if (newMonitorCount != monitorCount && newMonitorCount > 1)
                {
                    try
                    {
                        updatingWindows = true;
                        //lock (lockObject)
                        {
                            MonitorConfiguration config;
                            if (monitorConfigurations.TryGetValue(newMonitorCount, out config))
                            {
                                System.Threading.Thread.Sleep(3000);
                                RestoreWindowPositions(config.Windows);
                            }
                        }
                    }
                    finally
                    {
                        updatingWindows = false;
                    }
                }

                monitorCount = newMonitorCount;
            }
            catch (Exception ex)
            {
                logger.WriteLine(ex.ToString());
            }
            finally
            {
                monitorCount = newMonitorCount;
            }
        }

        public void RestoreWindowPositions(List<DesktopWindow> windows)
        {
            foreach (var window in windows)
            {
                if (!WindowHelpers.SetWindowPlacement(window.HWnd, window.Placement))
                {
                    var errorCode = Marshal.GetLastWin32Error();
                    logger.WriteLine($"Failed to place window: {window.WindowText}, Last error: {errorCode}");
                }
                System.Threading.Thread.Sleep(100);
            }
        }

        //private void CheckForDisplayUpdates()
        //{
        //    logger.WriteLine($"Screens: {Screen.AllScreens.Length}, Monitors: {SystemInformation.MonitorCount}");
        //    //var screenCount = Screen.AllScreens.Length;
        //    var screenCount = SystemInformation.MonitorCount;
        //    System.Diagnostics.Debug.WriteLine($"Screen count: {screenCount}");

        //    if (screenCount > previousScreenCount)
        //    {
        //        var json = System.IO.File.ReadAllText(windowPlacementJsonFile);
        //        var screenConfigurations = ParseScreenConfigurationJson(json);

        //        var screenConfig = screenConfigurations?.FirstOrDefault(cfg => cfg.ScreenCount == screenCount);

        //        if (screenConfig != null)
        //        {
        //            foreach (var windowPlacement in screenConfig.WindowPlacements)
        //            {
        //                var windowHandles = WindowHelpers.FindWindowsWithText(windowPlacement.WindowText);
        //                if (windowHandles?.Count == 1)
        //                {
        //                    WindowHelpers.MoveWindowScreen(windowHandles.First(), windowPlacement.ScreenAdjustment);
        //                }
        //            }
        //        }
        //    }
        //}

        internal static List<MonitorConfiguration> ParseScreenConfigurationJson(string screenConfigJson)
        {
            var screenConfigs = JsonConvert.DeserializeObject<List<MonitorConfiguration>>(screenConfigJson);
            return screenConfigs;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    SystemEvents.DisplaySettingsChanged -= SystemEvents_DisplaySettingsChanged;
                    saveWindowsTimer?.Dispose();
                }

                disposedValue = true;
            }
        }
        
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}
