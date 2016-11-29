using System;

namespace GpioChip.Net
{
    using System.Collections.Generic;
    using System.Threading;

    using Renci.SshNet;

    public class SshGpioInterface : IGpioInterface
    {
        private readonly SshClient client;
        private readonly ushort Pin0Index;

        private readonly Dictionary<short, PinSubscription> pinesSubscribed;

        public SshGpioInterface(string host, string username = "root", string password = "chip")
        {
            this.pinesSubscribed = new Dictionary<short, PinSubscription>();
            this.client = new SshClient(host, username, password);
            this.client.Connect();

            this.Pin0Index = this.GetPinBase();
            new Thread(this.CheckValueChanged).Start();
        }

        private ushort GetPinBase()
        {
            var command =  this.client.RunCommand("cat $(dirname $(grep -l pcf8574a /sys/class/gpio/*/*label))/base");

            ushort pinbase;
            UInt16.TryParse(command.Result, out pinbase);

            return pinbase;
        }

        public void Init(short pin)
        {
            this.client.RunCommand($"sh -c 'echo {Pin0Index + pin} > /sys/class/gpio/export'");
        }

        public void SetDirection(short pin, Direction direction)
        {
            this.client.RunCommand($"sh -c 'echo {direction.ToString().ToLower()} > /sys/class/gpio/gpio{Pin0Index + pin}/direction'");
        }

        public Direction GetDirection(short pin)
        {
            var command = this.client.RunCommand($"cat /sys/class/gpio/gpio{Pin0Index + pin}/direction");

            return (Direction)Enum.Parse(typeof(Direction), command.Result, true);
        }

        public void SetValue(short pin, short value)
        {
            this.client.RunCommand($"sh -c 'echo {value} > /sys/class/gpio/gpio{Pin0Index + pin}/value'");
        }

        public short GetValue(short pin)
        {
            var command = this.client.RunCommand($"cat /sys/class/gpio/gpio{Pin0Index + pin}/value");
            return Int16.Parse(command.Result);
        }

        public void SubscribeToValueChanged(short pinNumber, Action<short> valueChangedCallback)
        {
            lock (this.pinesSubscribed)
            {
                if (this.pinesSubscribed.ContainsKey(pinNumber) == false)
                {
                    this.pinesSubscribed.Add(pinNumber, new PinSubscription(this.GetValue(pinNumber)));
                }

                this.pinesSubscribed[pinNumber].OnValueChanged += valueChangedCallback; 
            }
        }

        public void UnsubscribeToValueChanged(short pinNumber, Action<short> valueChangedCallback)
        {
            lock (this.pinesSubscribed)
            {
                if (this.pinesSubscribed.ContainsKey(pinNumber))
                {
                    this.pinesSubscribed[pinNumber].OnValueChanged -= valueChangedCallback;
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
            this.client.RunCommand($"sh -c 'echo {Pin0Index + pin} > /sys/class/gpio/unexport'");
        }

        public void Dispose()
        {
            this.client.Disconnect();
            this.client.Dispose();
        }
    }
}
