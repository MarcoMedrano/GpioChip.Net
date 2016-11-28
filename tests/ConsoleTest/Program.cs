using System;

namespace GpioChip.Net.tests.ConsoleTest
{

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Running GPIO");
            using (var gpio = new Gpio(new SshGpioInterface("192.168.0.109")))
            {
                Console.WriteLine("Connected");
                for (int j = 0; j < 10; j++)
                {
                    for (short i = 0; i < 8; i++)
                    {
                        var pin = gpio.NewOutputPin(i);
                        if (j % 2 == 0)
                            pin.On();
                        else
                            pin.Off();
                    }
                }
            }
            Console.WriteLine("Finished");
            //Console.ReadKey();
        }
    }
}
