using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NoLock
{
    class MouseMove : IDisposable
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
        public DateTime LastMouseMoved { get; private set; }

        public event EventHandler MouseMoved;

        public MouseMove(int? interval)
        {
            if (interval.HasValue)
                Interval = interval.Value;

            moveMouseCancelationSource = new CancellationTokenSource();
            moveMouseCancelation = moveMouseCancelationSource.Token;

            moveMouse = Task.Factory.StartNew(() =>
            {
                while (!moveMouseCancelation.IsCancellationRequested)
                {
                    int cursorPosX = Cursor.Position.X;
                    int cursorPosY = Cursor.Position.Y;
                    int cursorPosMove = cursorPosX > 0 ? -1 : 1;
                    Cursor.Position = new Point(cursorPosX + cursorPosMove, cursorPosY);
                    Thread.Yield();
                    //Cursor.Position = new Point(cursorPosX + cursorPosMove * -1, cursorPosY);
                    onMouseMoved();
                    moveMouseCancelation.WaitHandle.WaitOne(Interval);
                }

            }, moveMouseCancelation);
        }

        private void onMouseMoved()
        {
            LastMouseMoved = DateTime.Now;
            if (MouseMoved != null)
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
