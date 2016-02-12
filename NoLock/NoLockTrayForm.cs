using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NoLock
{
    class NoLockTrayForm : Form
    {
        private MouseMovement mouse;
        private NotifyIcon trayIcon;
        private ContextMenu trayMenu;
        private MenuItem infoMenuItem;

        public NoLockTrayForm(int? interval = null)
        {
            trayMenu = new ContextMenu();
            infoMenuItem = new MenuItem() { Enabled = false };
            trayMenu.MenuItems.Add(infoMenuItem);
            trayMenu.MenuItems.Add("Exit", OnExit);

            trayIcon = new NotifyIcon();
            trayIcon.Text = "NoLock";
            trayIcon.Icon = new Icon(Icon.ExtractAssociatedIcon(Application.ExecutablePath), 40, 40);
            trayIcon.ContextMenu = trayMenu;
            
            mouse = new MouseMovement(interval);
            infoMenuItem.Text = string.Concat("Interval: ", (mouse.Interval / 1000d).ToString(), "s");
            mouse.MouseMoved += mouse_MouseMoved;
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

        private void mouse_MouseMoved(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                string originalText = infoMenuItem.Text;
                infoMenuItem.Text += "*";
                System.Threading.Thread.Sleep(500);
                infoMenuItem.Text = originalText;
            });
        }
    }
}
