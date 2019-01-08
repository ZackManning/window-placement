using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowPlacementLib;

namespace WindowDisplayApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var displayWatcher = new DisplayWatcher();

            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
        }
    }
}
