namespace GpioChip.Net
{
    using System;

    public class Pin : IDisposable
    {
        protected readonly IGpioInterface gpioInterface;

        public Pin(short pinNumber, IGpioInterface gpioInterface, PinBase pinBase)
        {
            this.PinNumber = pinNumber;
            this.RawPinNumber = (short)(pinBase + pinNumber);
            this.gpioInterface = gpioInterface;
            this.gpioInterface.Init(this.RawPinNumber);
        }

        public short PinNumber { get; private set; }

        public short RawPinNumber { get; private set; }

        public short Value
        {
            get
            {
                return this.gpioInterface.GetValue(this.RawPinNumber);
            }
            set
            {
                if(this.Direction == Direction.In)
                    throw new InvalidOperationException($"Can not set : {value} to pin {this.PinNumber} as direction is input.");
                this.gpioInterface.SetValue(this.RawPinNumber, value);
            }
        }

        public Direction Direction
        {
            get
            {
                return this.gpioInterface.GetDirection(this.RawPinNumber);
            }
            set
            {
                this.gpioInterface.SetDirection(this.RawPinNumber, value);
            }
        }

        public void Dispose()
        {
            this.gpioInterface.Dispose(this.RawPinNumber);
        }
    }
}
