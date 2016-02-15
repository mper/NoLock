using System;
using System.Drawing;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NoLock
{
    class NoLockTrayForm : Form
    {
        private MouseMove mouse;
        private NotifyIcon trayIcon;
        private ContextMenu trayMenu;
        private MenuItem infoMenuItem;
        private Timer trayMenuDisplayTimer;

        public NoLockTrayForm(int? interval = null)
        {
            trayMenu = new ContextMenu();
            trayMenu.Popup += TrayMenu_Popup;
            trayMenu.Collapse += TrayMenu_Collapse;
            infoMenuItem = new MenuItem() { Enabled = false };
            trayMenu.MenuItems.Add(infoMenuItem);
            trayMenu.MenuItems.Add("Exit", OnExit);

            trayIcon = new NotifyIcon();
            trayIcon.Text = "NoLock";
            trayIcon.Icon = new Icon(Icon.ExtractAssociatedIcon(Application.ExecutablePath), 40, 40);
            trayIcon.ContextMenu = trayMenu;
            trayIcon.MouseClick += trayIcon_MouseClick;
            
            mouse = new MouseMove(interval);
            infoMenuItem.Text = string.Concat("Interval: ", (mouse.Interval / 1000d).ToString(), "s");
            mouse.MouseMoved += mouse_MouseMoved;

            trayMenuDisplayTimer = new Timer();
            trayMenuDisplayTimer.Interval = 1000;
            trayMenuDisplayTimer.Tick += trayMenuDisplayTimer_Tick;
        }

        private void trayIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
                typeof(NotifyIcon).GetMethod("ShowContextMenu",
                    BindingFlags.Instance | BindingFlags.NonPublic).Invoke(trayIcon, null);
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

        private void TrayMenu_Popup(object sender, EventArgs e)
        {
            trayMenuDisplayTimer_Tick(null, null);
            trayMenuDisplayTimer.Start();
        }

        private void TrayMenu_Collapse(object sender, EventArgs e)
        {
            trayMenuDisplayTimer.Stop();
        }

        private void trayMenuDisplayTimer_Tick(object sender, EventArgs e)
        {
            int mouseMoveCountdown = mouse.Interval / 1000 - 
                (int)(DateTime.Now - mouse.LastMouseMoved).TotalSeconds;
            infoMenuItem.Text = string.Format("Interval: {0}s ({1}){2}",
                (mouse.Interval / 1000d).ToString(), mouseMoveCountdown,
                mouseMoveCountdown == mouse.Interval / 1000 ? "*" : "");
        }

        private void OnExit(object sender, EventArgs e)
        {
            mouse.Dispose();
            Close();
        }

        private void mouse_MouseMoved(object sender, EventArgs e)
        {
            // do something
        }
    }
}
