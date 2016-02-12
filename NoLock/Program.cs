using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace NoLock
{
    class Program
    {
        static void Main(string[] args)
        {
            int? interval = null;
            int tmp;
            if (args.Length > 0 && int.TryParse(args[0], out tmp))
                interval = tmp;

            Application.Run(new NoLockTrayForm(interval));
        }
    }
}
