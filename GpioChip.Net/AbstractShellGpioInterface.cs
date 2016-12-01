namespace GpioChip.Net
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    public abstract class AbstractShellGpioInterface : IGpioInterface
    {
        protected readonly Dictionary<short, PinSubscription> pinesSubscribed;
        private Thread threadToCheckEvents = null;

        protected AbstractShellGpioInterface()
        {
            this.pinesSubscribed = new Dictionary<short, PinSubscription>();

            this.threadToCheckEvents = new Thread(this.CheckValueChanged);
            this.threadToCheckEvents.Start();
        }

        protected abstract AbstractShellCommandResult RunCommand(string command);

        public PinBase GetDefaultPinBase()
        {
            var command =  this.RunCommand("cat $(dirname $(grep -l pcf8574a /sys/class/gpio/*/*label))/base");

            ushort pinbase;
            UInt16.TryParse(command.Result, out pinbase);

            return (PinBase)pinbase;
        }

        public virtual void Init(short pin)
        {
            this.RunCommand($"echo {pin} > /sys/class/gpio/export");

            lock (this.pinesSubscribed)
            {
                if (this.pinesSubscribed.ContainsKey(pin) == false)
                {
                    this.pinesSubscribed.Add(pin, new PinSubscription(this.GetValue(pin)));
                }
            }
        }

        public void SetDirection(short pin, Direction direction)
        {
            var res = this.RunCommand($"echo {direction.ToString().ToLower()} > /sys/class/gpio/gpio{pin}/direction");
            if (res.ExitCode == 1) throw new Exception($"Could not change direction of raw pin {pin} \n With internal message: {res.ErrorMessage}");
        }

        public Direction GetDirection(short pin)
        {
            var command = this.RunCommand($"cat /sys/class/gpio/gpio{pin}/direction");

            return (Direction)Enum.Parse(typeof(Direction), command.Result, true);
        }

        public void SetValue(short pin, short value)
        {
            var res = this.RunCommand($"echo {value} > /sys/class/gpio/gpio{pin}/value");
            if (res.ExitCode == 1) throw new Exception($"Could not change value of raw pin {pin} \n With internal message: {res.ErrorMessage}");
        }

        public short GetValue(short pin)
        {
            var command = this.RunCommand($"cat /sys/class/gpio/gpio{pin}/value");
            return Int16.Parse(command.Result);
        }

        public void SubscribeToValueChanged(short pin, Action<short> valueChangedCallback)
        {
            this.pinesSubscribed[pin].OnValueChanged += valueChangedCallback; 
        }

        public void UnsubscribeToValueChanged(short pin, Action<short> valueChangedCallback)
        {
            lock (this.pinesSubscribed)
            {
                if (this.pinesSubscribed.ContainsKey(pin))
                {
                    this.pinesSubscribed[pin].OnValueChanged -= valueChangedCallback;
                } 
            }
        }

        private void CheckValueChanged()
        {
            while (this.threadToCheckEvents != null)
            {
                //Console.WriteLine($"Memory used {System.Diagnostics.Process.GetCurrentProcess().WorkingSet64/1024/1024} MB");
                lock (this.pinesSubscribed)
                {
                    foreach (KeyValuePair<short, PinSubscription> keyValue in this.pinesSubscribed)
                    {
                        if (this.pinesSubscribed[keyValue.Key].AreEventsSubscribed == false) continue;

                        var newValue = this.GetValue(keyValue.Key);

                        if (newValue != keyValue.Value.LastValue)
                        {
                            keyValue.Value.LastValue = newValue;
                            keyValue.Value.RaiseEvent(newValue);
                        }
                    }
                }
            }
        }

        public void Dispose(short pin)
        {
            this.RunCommand($"echo {pin} > /sys/class/gpio/unexport");
        }

        public void Dispose()
        {
            this.threadToCheckEvents = null;
            lock (this.pinesSubscribed)
            {
                foreach (KeyValuePair<short, PinSubscription> keyValue in this.pinesSubscribed)
                {
                    keyValue.Value.RemoveEventsSubscribed();
                    this.Dispose(keyValue.Key);
                }
            }
        }
    }
}
