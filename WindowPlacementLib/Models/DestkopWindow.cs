using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowPlacementLib.Models
{
    public class DesktopWindow
    {
        public IntPtr HWnd { get; set; }
        public string WindowText { get; set; }
        public WindowHelpers.Rect Rect { get; set; }
        public WindowHelpers.WindowPlacement Placement { get; set; }

        public DesktopWindow(IntPtr hWnd, string windowText, WindowHelpers.Rect rect, WindowHelpers.WindowPlacement placement)
        {
            HWnd = hWnd;
            WindowText = windowText;
            Rect = rect;
            Placement = placement;
        }

        public override string ToString()
        {
            return $"{WindowText}";
        }
    }
}
