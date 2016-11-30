using System;

namespace GpioChip.Net
{
    using System.Collections.Generic;
    using System.Threading;

    using Renci.SshNet;

    public class SshGpioInterface : IGpioInterface
    {
        private readonly SshClient client;

        private readonly Dictionary<short, PinSubscription> pinesSubscribed;

        public SshGpioInterface(string host, string username = "root", string password = "chip")
        {
            this.pinesSubscribed = new Dictionary<short, PinSubscription>();
            this.client = new SshClient(host, username, password);
            this.client.Connect();

            new Thread(this.CheckValueChanged).Start();
        }

        public PinBase GetDefaultPinBase()
        {
            var command =  this.client.RunCommand("cat $(dirname $(grep -l pcf8574a /sys/class/gpio/*/*label))/base");

            ushort pinbase;
            UInt16.TryParse(command.Result, out pinbase);

            return (PinBase)pinbase;
        }

        public void Init(short pin)
        {
            this.client.RunCommand($"sh -c 'echo {pin} > /sys/class/gpio/export'");

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
            var res = this.client.RunCommand($"sh -c 'echo {direction.ToString().ToLower()} > /sys/class/gpio/gpio{pin}/direction'");
            if (res.ExitStatus == 1) throw new Exception($"Could not change direction of raw pin {pin} \n With internal message: {res.Error}");
        }

        public Direction GetDirection(short pin)
        {
            var command = this.client.RunCommand($"cat /sys/class/gpio/gpio{pin}/direction");

            return (Direction)Enum.Parse(typeof(Direction), command.Result, true);
        }

        public void SetValue(short pin, short value)
        {
            var res = this.client.RunCommand($"sh -c 'echo {value} > /sys/class/gpio/gpio{pin}/value'");
            if (res.ExitStatus == 1) throw new Exception($"Could not change value of raw pin {pin} \n With internal message: {res.Error}");
        }

        public short GetValue(short pin)
        {
            var command = this.client.RunCommand($"cat /sys/class/gpio/gpio{pin}/value");
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
            while (true)
            {
                lock (this.pinesSubscribed)
                {
                    foreach (KeyValuePair<short, PinSubscription> keyValue in this.pinesSubscribed)
                    {
                        if (this.pinesSubscribed[keyValue.Key].AreEventsSubscribed == false ) continue;

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
            this.client.RunCommand($"sh -c 'echo {pin} > /sys/class/gpio/unexport'");
        }

        public void Dispose()
        {
            lock (this.pinesSubscribed)
            {
                foreach (KeyValuePair<short, PinSubscription> keyValue in this.pinesSubscribed)
                {
                    this.Dispose(keyValue.Key);
                }
            }

            this.client.Disconnect();
            this.client.Dispose();
        }
    }
}
