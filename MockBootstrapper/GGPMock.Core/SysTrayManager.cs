using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;

namespace GGPMockBootstrapper
{

    public interface ISysTrayManager
    {
        bool Enabled { get; set; }
    }


    public class VoidSysTrayManager : ISysTrayManager
    {

        #region ISysTrayManager Members

        public bool Enabled
        {
            get;
            set;
        }

        #endregion
    }

    public class WpfSysTrayManager : ISysTrayManager
    {
        System.Windows.Forms.NotifyIcon _systrayIcon;
        ClickOnceBackgroundUpdater _appUpdater;

        public WpfSysTrayManager(Window mainWindow)
        {
            
            this.MainWindow = mainWindow;
            this.MainWindow.Closing += MainWindow_Closing;
                        

            InitSystrayIcon();

            _appUpdater = new ClickOnceBackgroundUpdater(CheckForUpdatesInterval);
            _appUpdater.ApplicationUpdated += _appUpdater_ApplicationUpdated;
        }



        private static bool IsFlorin()
        {
            return Environment.UserName == "florinia" || Environment.UserName == "florin.iacob" || Environment.UserName == "Florin";
        }

        private TimeSpan CheckForUpdatesInterval
        {
            get
            {
                if (IsFlorin())
                    return TimeSpan.FromSeconds(30);
                else
                    return TimeSpan.FromHours(1);
            }
        }

        private TimeSpan ReminderInterval
        {
            get
            {
                if (IsFlorin())
                    return TimeSpan.FromSeconds(15);
                else
                    return TimeSpan.FromHours(2);
            }
        }

        System.Timers.Timer _reminderTimer = null;

        void _appUpdater_ApplicationUpdated(object sender, EventArgs e)
        {
            ShowBalloonTip(NewVersionMessage);

            if (_reminderTimer == null)
            {
                _reminderTimer = new System.Timers.Timer(ReminderInterval.TotalMilliseconds);
                _reminderTimer.Elapsed += _reminderTimer_Elapsed;
                _reminderTimer.Start();
            }
        }

        void _reminderTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            ShowBalloonTip(string.Format("REMINDER! {0}", NewVersionMessage));
        }


        private string NewVersionMessage
        {
            get
            {
                return string.Format("GGP Mock version {0} is available! You need to restart the application in order to take advantages of this new version.",
                                            ApplicationDeployment.CurrentDeployment.UpdatedVersion.ToString());
            }
        }
        
        private void ManualExit()
        {
            this.Enabled = false;
            HideSysTrayIcon();
            this.MainWindow.Close();
        }

        private void HideSysTrayIcon()
        {
            _systrayIcon.Visible = false;
            _systrayIcon.Icon = null;
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
            if (this.Enabled)
            {
                e.Cancel = true;
                ShowInSysTray();
                this.MainWindow.Visibility = Visibility.Collapsed;
            }
            else
            {
                HideSysTrayIcon();
                _appUpdater.Stop();
            }
            
        }

        Window MainWindow { get; set; }

        private void InitSystrayIcon()
        {
            _systrayIcon = new System.Windows.Forms.NotifyIcon();
            _systrayIcon.Text = "GGP Mock";
            
            _systrayIcon.Icon = GGPMockBootstrapper.Properties.Resources.GGPMockBootstrapper;
            _systrayIcon.DoubleClick += _systrayIcon_DoubleClick;
            _systrayIcon.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            _systrayIcon.ContextMenuStrip.Items.Add("Show", null, (sndr, args) => ShowMainWindow());
            //commented because in Win 10 the NotificationIcon doesn't work anymore after restarting the application.
            //_systrayIcon.ContextMenuStrip.Items.Add("Restart application", null, (sndr, args) => RestartApplication());
            _systrayIcon.ContextMenuStrip.Items.Add("-");
            _systrayIcon.ContextMenuStrip.Items.Add("Exit", null, (sndr, args) => ManualExit());
            ShowInSysTray();
        }

       

//this doesn't work well on windows 10
        private void RestartApplication()
        {
            ManualExit();

            string url = null;
            
            if(ApplicationDeployment.IsNetworkDeployed)
            {
                url = ApplicationDeployment.CurrentDeployment.UpdatedApplicationFullName.Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            }

            if (!string.IsNullOrEmpty(url))
            {
                var processStartInfo = new ProcessStartInfo("rundll32.exe");
                processStartInfo.Arguments = "dfshim.dll,ShOpenVerbApplication " + url;
                processStartInfo.UseShellExecute = true;
                processStartInfo.Verb = "runas";
                System.Diagnostics.Process.Start(processStartInfo);
            }

            

           
          
        }

        void _systrayIcon_DoubleClick(object sender, EventArgs e)
        {
            ShowMainWindow();
        }


        private void ShowMainWindow()
        {
            this.MainWindow.Visibility = System.Windows.Visibility.Visible;
            this.MainWindow.WindowState = System.Windows.WindowState.Maximized;
            this.MainWindow.BringIntoView();
        }

        private void ShowInSysTray()
        {
            _systrayIcon.Visible = true;   
            ShowBalloonTip("I'm here running and looking for updates!");
        }

        private void ShowBalloonTip(string message)
        {
            _systrayIcon.ShowBalloonTip(5000, "GGP Mock", message, System.Windows.Forms.ToolTipIcon.Info);
        }

        #region ISysTrayManager Members

        private bool _enabled = false;
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        #endregion
    }

    
}
