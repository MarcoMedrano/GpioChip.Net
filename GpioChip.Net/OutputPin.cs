namespace GpioChip.Net
{
    public class OutputPin : Pin
    {
        public OutputPin(short pinNumber, IGpioInterface gpioInterface): base(pinNumber, gpioInterface)
        {
            this.Direction = Direction.Out;
        }

        public void On() => this.Value = 1;

        public void Off() => this.Value = 0;
    }
}
