using System;
using System.Drawing;
using System.Windows.Forms;

namespace NoLock
{
    class NoLockTrayForm : Form
    {
        private MouseMovement mouse;
        private NotifyIcon trayIcon;
        private ContextMenu trayMenu;

        public NoLockTrayForm(int? interval = null)
        {
            trayMenu = new ContextMenu();
            MenuItem intervalMenuItem = new MenuItem() { Enabled = false };
            trayMenu.MenuItems.Add(intervalMenuItem);
            trayMenu.MenuItems.Add("Exit", OnExit);

            trayIcon = new NotifyIcon();
            trayIcon.Text = "NoLock";
            trayIcon.Icon = new Icon(Icon.ExtractAssociatedIcon(Application.ExecutablePath), 40, 40);
            trayIcon.ContextMenu = trayMenu;
            
            mouse = new MouseMovement(interval);
            intervalMenuItem.Text = string.Concat("Interval: ", (mouse.Interval / 1000d).ToString(), "s");
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

        private void OnExit(object sender, EventArgs e)
        {
            mouse.Dispose();
            Close();
        }
    }
}
