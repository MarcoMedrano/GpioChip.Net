using System;

namespace GpioChip.Net
{
    public class PinSubscription
    {
        public PinSubscription(short lastValue)
        {
            this.LastValue = lastValue;
        }

        public short LastValue { get; set; }

        public event Action<short> OnValueChanged;

        public void RaiseEvent(short newValue)
        {
            this.OnValueChanged?.Invoke(newValue);
        }
    }
}
