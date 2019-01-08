using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using WindowPlacementLib;

namespace WindowDisplayService
{
    public partial class WindowPlacementService : ServiceBase
    {
        private DisplayWatcher displayWatcher;

        public WindowPlacementService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Start();
        }

        public void Start()
        {
            displayWatcher = new DisplayWatcher();
        }

        protected override void OnStop()
        {
            displayWatcher?.Dispose();
        }
    }
}
