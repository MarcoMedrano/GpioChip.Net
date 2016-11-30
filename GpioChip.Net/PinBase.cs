namespace GpioChip.Net
{
    public enum PinBase
    {
        ABSOLUTE = 0,
        XIO_P0_408 = 408,
        XIO_P0_1016 = 1016,
        PD2 = (32 * 3) + 2, // A=0=>32 -> D=3=>(32*3)
        PE0 = (32*4),
        LCD_D2 = PD2, 
        CSIPCK = PE0
    }
}
