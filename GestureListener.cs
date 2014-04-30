using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Leap;

namespace waterwars23._0
{
    public class GestureListener : Listener
    {
        /// <summary>
        /// Delegate LeapListenerEventHandler
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="LeapListenerEventArgs" /> instance containing the event data.</param>
        public delegate void LeapListenerEventHandler(object sender, LeapListenerEventArgs e);
        /// <summary>
        /// Occurs when a new frame is received.
        /// </summary>
        public event LeapListenerEventHandler OnFrameChanged;
        /// <summary>
        /// Occurs when the leap unit has initialized.
        /// </summary>
        public event LeapListenerEventHandler OnInitialized;
        /// <summary>
        /// Occurs when the leap unit has connected.
        /// </summary>
        public event LeapListenerEventHandler OnConnected;
        /// <summary>
        /// Occurs when the leap unit has disconnected.
        /// </summary>
        public event LeapListenerEventHandler OnDisconnected;
        /// <summary>
        /// Occurs when the leap software has exited.
        /// </summary>
        public event LeapListenerEventHandler OnExited;

        /// <summary>
        /// Called when the listener has initialized.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public override void OnInit(Controller controller)
        {
            if (OnInitialized != null)
            {
                OnInitialized(this, new LeapListenerEventArgs(controller));
            }
        }

        /// <summary>
        /// Called when the controller has connected.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public override void OnConnect(Controller controller)
        {
            controller.EnableGesture(Gesture.GestureType.TYPECIRCLE);
            controller.EnableGesture(Gesture.GestureType.TYPEKEYTAP);
            controller.EnableGesture(Gesture.GestureType.TYPESCREENTAP);
            controller.EnableGesture(Gesture.GestureType.TYPESWIPE);
            if (OnConnected != null)
            {
                Console.WriteLine("Connected");
                OnConnected(this, new LeapListenerEventArgs(controller));
            }
        }

        /// <summary>
        /// Called when the controller has disconnected.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public override void OnDisconnect(Controller controller)
        {
            if (OnDisconnected != null)
            {
                OnDisconnected(this, new LeapListenerEventArgs(controller));
            }
        }

        /// <summary>
        /// Called when the leap software has exited.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public override void OnExit(Controller controller)
        {
            if (OnExited != null)
            {
                OnExited(this, new LeapListenerEventArgs(controller));
            }
        }

        /// <summary>
        /// Called when new frame data is available.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public override void OnFrame(Controller controller)
        {
            if (OnFrameChanged != null)
            {
                OnFrameChanged(this, new LeapListenerEventArgs(controller));
            }
        }
    }
}
