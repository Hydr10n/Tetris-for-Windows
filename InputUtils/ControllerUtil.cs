using System.Threading;
using SharpDX.XInput;
using Timer = System.Timers.Timer;

namespace Hydr10n.InputUtils
{
    class ControllerUtil
    {
        public delegate void ConnectionChangedEventHandler(ControllerUtil sender, bool isConnected);
        public delegate void PollEventHandler(ControllerUtil sender);

        private readonly Timer Timer = new Timer();

        private bool isPreviouslyConnected;

        private double pollingRate;
        public double PollingRate
        {
            get => pollingRate;
            set
            {
                pollingRate = value;
                Timer.Interval = 1000 / value;
            }
        }

        public bool EnablePolling { get => Timer.Enabled; set => Timer.Enabled = value; }

        public Controller Controller { get; private set; }

        public event ConnectionChangedEventHandler ConnectionChanged;
        public event PollEventHandler Poll;

        public ControllerUtil(UserIndex userIndex = UserIndex.Any, double pollingRate = 125)
        {
            Controller = new Controller(userIndex);
            isPreviouslyConnected = Controller.IsConnected;
            PollingRate = pollingRate;
            Timer.Elapsed += (sender, e) =>
            {
                bool isConnected = Controller.IsConnected;
                if (isConnected != isPreviouslyConnected)
                {
                    isPreviouslyConnected = isConnected;
                    ConnectionChanged?.Invoke(this, isConnected);
                }
                if (isConnected)
                    Poll?.Invoke(this);
            };
        }

        public void Vibrate(Vibration vibration, int millionseconds) => new Thread(() =>
        {
            Controller.SetVibration(vibration);
            Thread.Sleep(millionseconds);
            Controller.SetVibration(new Vibration());
        }).Start();
    }
}
