using System;

namespace GpioChip.Net
{
    public class PinSubscription
    {
        public PinSubscription(short lastValue)
        {
            this.LastValue = lastValue;
        }

        public event Action<short> OnValueChanged;

        public short LastValue { get; set; }

        public bool AreEventsSubscribed => this.OnValueChanged != null;

        public void RaiseEvent(short newValue)
        {
            this.OnValueChanged?.Invoke(newValue);
        }
    }
}
