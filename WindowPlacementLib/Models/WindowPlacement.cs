using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowPlacementLib.Models
{
    public class WindowPlacement
    {
        public string WindowText { get; set; }
        public short ScreenAdjustment { get; set; }

        public override string ToString()
        {
            return $"{WindowText}, {ScreenAdjustment}";
        }
    }
}
