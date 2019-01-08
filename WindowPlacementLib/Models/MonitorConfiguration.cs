using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowPlacementLib.Models
{
    public class MonitorConfiguration
    {
        public int MonitorCount { get; set; }
        public List<DesktopWindow> Windows { get; set; } = new List<DesktopWindow>();

        public override string ToString()
        {
            return $"{MonitorCount} screens";
        }
    }
}
