namespace GpioChip.Net
{
    public class OutputPin : Pin
    {
        public OutputPin(short pinNumber, IGpioInterface gpioInterface, PinBase pinBase): base(pinNumber, gpioInterface, pinBase)
        {
            this.Direction = Direction.Out;
        }

        public void On() => this.Value = 1;

        public void Off() => this.Value = 0;
    }
}
