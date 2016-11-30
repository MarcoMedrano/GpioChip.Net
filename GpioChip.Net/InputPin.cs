using System;

namespace GpioChip.Net
{
    public class InputPin : Pin
    {
        private event Action<short> onValueChanged;

        public InputPin(short pinNumber, IGpioInterface gpioInterface, PinBase pinBase) : base(pinNumber, gpioInterface, pinBase)
        {
            this.Direction = Direction.In;
            this.gpioInterface.SubscribeToValueChanged(this.PinNumber, this.ValueChanged);
        }


        public event Action<short> OnValueChanged
        {
            add
            {
                this.onValueChanged += value;
            }
            remove
            {
                this.onValueChanged -= value;
            }
        }

        private void ValueChanged(short value)
        {
            this.onValueChanged?.Invoke(value);
        }

        // Is it neede?
        //private new void Dispose()
        //{
        //    this.gpioInterface.UnsubscribeToValueChanged(this.PinNumber, this.ValueChanged);
        //    base.Dispose();
        //}
    }
}
