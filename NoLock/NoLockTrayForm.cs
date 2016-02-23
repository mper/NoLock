using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace NoLock
{
    class NoLockTrayForm : Form
    {
        private NotifyIcon trayIcon;
        private ContextMenu trayMenu;
        private MenuItem infoMenuItem;

        public NoLockTrayForm(int? interval = null)
        {
            trayMenu = new ContextMenu();
            infoMenuItem = new MenuItem() { Enabled = false, Text = "NoLock" };
            trayMenu.MenuItems.Add(infoMenuItem);
            trayMenu.MenuItems.Add("Exit", OnExit);

            trayIcon = new NotifyIcon();
            trayIcon.Text = "NoLock";
            trayIcon.Icon = new Icon(Icon.ExtractAssociatedIcon(Application.ExecutablePath), 40, 40);
            trayIcon.ContextMenu = trayMenu;
            trayIcon.MouseClick += trayIcon_MouseClick;


            NoLockWinApi.SetExecutionState();
        }

        protected override void OnLoad(EventArgs e)
        {
            // Show tray, hide form window
            trayIcon.Visible = true;
            Visible = false;
            ShowInTaskbar = false;

            base.OnLoad(e);
        }

        protected override void Dispose(bool disposing)
        {
            trayIcon.Visible = false;
            base.Dispose(disposing);
        }

        /// <summary>
        /// Enable tray context menu on left mouse click
        /// </summary>
        private void trayIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                typeof(NotifyIcon).GetMethod("ShowContextMenu",
                    BindingFlags.Instance | BindingFlags.NonPublic).Invoke(trayIcon, null);
        }

        private void OnExit(object sender, EventArgs e)
        {
            Close();
        }
    }
}
