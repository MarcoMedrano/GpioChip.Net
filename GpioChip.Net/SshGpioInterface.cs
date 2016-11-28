﻿using System;

namespace GpioChip.Net
{
    using System.Collections.Generic;
    using System.Threading;

    using Renci.SshNet;

    public class SshGpioInterface : IGpioInterface
    {
        private readonly SshClient client;
        private const int Pin0Index = 408;

        private Dictionary<short, PinSubscription> pinesSubscribed;

        public SshGpioInterface(string host, string username = "root", string password = "chip")
        {
            this.pinesSubscribed = new Dictionary<short, PinSubscription>();
            this.client = new SshClient(host, username, password);
            this.client.Connect();

            new Thread(this.CheckValueChanged).Start();
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
            if (this.pinesSubscribed.ContainsKey(pinNumber) == false)
            {
                this.pinesSubscribed.Add(pinNumber, new PinSubscription(this.GetValue(pinNumber)));
            }

            this.pinesSubscribed[pinNumber].OnValueChanged += valueChangedCallback;
        }

        public void UnsubscribeToValueChanged(short pinNumber, Action<short> valueChangedCallback)
        {
            if (this.pinesSubscribed.ContainsKey(pinNumber))
            {
                this.pinesSubscribed[pinNumber].OnValueChanged -= valueChangedCallback;
            }

        }

        private void CheckValueChanged()
        {
            while (true)
            {
                foreach (KeyValuePair<short, PinSubscription> keyValue in this.pinesSubscribed)
                {
                    var newValue = this.GetValue(keyValue.Key);

                    if (newValue != keyValue.Value.LastValue)
                    {
                        keyValue.Value.RaiseEvent(newValue);
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
