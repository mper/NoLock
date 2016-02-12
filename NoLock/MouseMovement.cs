using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NoLock
{
    class MouseMovement : IDisposable
    {
        private CancellationTokenSource moveMouseCancelationSource;
        private CancellationToken moveMouseCancelation;
        private Task moveMouse;

        private int interval = 59000;
        public int Interval
        {
            get { return interval; }
            set { interval = value; }
        }

        public event EventHandler MouseMoved;

        public MouseMovement(int? interval)
        {
            if (interval.HasValue)
                Interval = interval.Value;

            moveMouseCancelationSource = new CancellationTokenSource();
            moveMouseCancelation = moveMouseCancelationSource.Token;

            moveMouse = Task.Factory.StartNew(() =>
            {
                while (!moveMouseCancelation.IsCancellationRequested)
                {
                    Cursor.Position = new Point(Cursor.Position.X - 1, Cursor.Position.Y);
                    Thread.Yield();
                    Cursor.Position = new Point(Cursor.Position.X + 1, Cursor.Position.Y);
                    onMouseMoved();
                    moveMouseCancelation.WaitHandle.WaitOne(Interval);
                }

            }, moveMouseCancelation);
        }

        private void onMouseMoved()
        {
            if(MouseMoved != null)
                MouseMoved(this, new EventArgs());
        }

        public void Dispose()
        {
            if (moveMouse != null)
            {
                moveMouseCancelationSource.Cancel();
                try
                {
                    moveMouse.Wait();
                }
                catch (AggregateException e)
                {
                    foreach (var v in e.InnerExceptions)
                        Console.WriteLine(e.Message + " " + v.Message);
                }
                finally
                {
                    moveMouseCancelationSource.Dispose();
                    moveMouse.Dispose();
                    moveMouse = null;
                }
            }
        }
    }
}
