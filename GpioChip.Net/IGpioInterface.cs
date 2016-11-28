namespace GpioChip.Net
{
    using System;

    public interface IGpioInterface : IDisposable
    {
        void Init(short pin);

        void SetDirection(short pin, Direction direction);

        Direction GetDirection(short pin);

        void SetValue(short pin, short value);

        short GetValue(short pin);

        void SubscribeToValueChanged(short pinNumber, Action<short> valueChangedCallback);

        void UnsubscribeToValueChanged(short pinNumber, Action<short> valueChanged);

        void Dispose(short pinNumber);
    }
}
