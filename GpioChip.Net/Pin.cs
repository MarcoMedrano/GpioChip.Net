namespace GpioChip.Net
{
    using System;

    public class Pin : IDisposable
    {
        protected readonly IGpioInterface gpioInterface;

        public Pin(short pinNumber, IGpioInterface gpioInterface)
        {
            this.PinNumber = pinNumber;
            this.gpioInterface = gpioInterface;
            this.gpioInterface.Init(this.PinNumber);
        }

        public short PinNumber { get; protected set; }

        public short Value
        {
            get
            {
                return this.gpioInterface.GetValue(this.PinNumber);
            }
            set
            {
                if(this.Direction == Direction.In)
                    throw new InvalidOperationException($"Can not set : {value} to pin {this.PinNumber} as direction is input.");
                this.gpioInterface.SetValue(this.PinNumber, value);
            }
        }

        public Direction Direction
        {
            get
            {
                return this.gpioInterface.GetDirection(this.PinNumber);
            }
            set
            {
                this.gpioInterface.SetDirection(this.PinNumber, value);
            }
        }

        public void Dispose()
        {
            this.gpioInterface.Dispose(this.PinNumber);
        }
    }
}
