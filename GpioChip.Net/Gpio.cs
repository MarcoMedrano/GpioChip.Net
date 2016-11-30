using System;

namespace GpioChip.Net
{
    public class Gpio : IDisposable
    {
        private readonly IGpioInterface gpioInterface;
        private readonly PinBase defaultPinBase;


        public Gpio(IGpioInterface gpioInterface, PinBase defaultPinBase)
        {
            this.gpioInterface = gpioInterface;
            this.defaultPinBase = defaultPinBase;
        }

        public Gpio(IGpioInterface gpioInterface)
        {
            this.gpioInterface = gpioInterface;
            this.defaultPinBase = gpioInterface.GetDefaultPinBase();
        }

        public Pin NewPin(short pinNumber, Direction direction = Direction.Out, short value = 0)
        {
            return this.NewPin(pinNumber, this.defaultPinBase, direction, value);
        }

        public Pin NewPin(short pinNumber, PinBase pinBase, Direction direction = Direction.Out, short value = 0)
        {
            var newPin = new Pin(pinNumber, this.gpioInterface, pinBase);
            newPin.Direction = direction;
            newPin.Value = value;

            return newPin;
        }

        public InputPin NewInputPin(short pinNumber)
        {
            return this.NewInputPin(pinNumber, this.defaultPinBase);
        }

        public InputPin NewInputPin(short pinNumber, PinBase pinBase)
        {
            return new InputPin(pinNumber, this.gpioInterface, pinBase);
        }

        public OutputPin NewOutputPin(short pinNumber)
        {
            return this.NewOutputPin(pinNumber, this.defaultPinBase);
        }

        public OutputPin NewOutputPin(short pinNumber, PinBase pinBase)
        {
            return new OutputPin(pinNumber, this.gpioInterface, pinBase);
        }

        public void Dispose()
        {
            this.gpioInterface.Dispose();
        }
    }
}
