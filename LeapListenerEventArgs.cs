using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leap;

namespace waterwars23._0
{
    public class LeapListenerEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the controller associated with the event.
        /// </summary>
        /// <value>The controller instance.</value>
        public Controller LmController { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LeapListenerEventArgs" /> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public LeapListenerEventArgs(Controller controller)
        {
            LmController = controller;
        }
    }
}
