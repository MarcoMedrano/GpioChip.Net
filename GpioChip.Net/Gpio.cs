using System;

namespace GpioChip.Net
{
    public class Gpio : IDisposable
    {
        private IGpioInterface gpioInterface;

        public Gpio(IGpioInterface gpioInterface)
        {
            this.gpioInterface = gpioInterface;
        }

        public Pin NewPin(short pinNumber, Direction direction = Direction.Out, short value = 0)
        {
            var newPin = new Pin(pinNumber, this.gpioInterface);
            newPin.Direction = direction;
            newPin.Value = value;

            return newPin;
        }

        public InputPin NewInputPin(short pinNumber)
        {
            return new InputPin(pinNumber, this.gpioInterface);
        }

        public OutputPin NewOutputPin(short pinNumber)
        {
            return new OutputPin(pinNumber, this.gpioInterface);
        }

        public void Dispose()
        {
            this.gpioInterface.Dispose();
        }
    }
}
