using System;

namespace GpioChip.Net.tests.ConsoleTest
{
    using System.Diagnostics;
    using System.Threading;

    public class Program
    {
        public static void Main(string[] args)
        {
            var stopWatch = Stopwatch.StartNew();
            Console.WriteLine("Running GPIO");
            TestOutput();
            //TestInput();
            Console.WriteLine($"Finished with: {stopWatch.Elapsed.TotalSeconds} seconds");
            Console.ReadKey();
        }

        private static void TestOutput()
        {
            using (var gpio = new Gpio(new ShellGpioInterface()))
           // using (var gpio = new Gpio(new SshGpioInterface("192.168.0.109")))
            {
                Console.WriteLine("Connected");
                for (short i = 0; i < 8; i++)
                {
                    var pin = gpio.NewOutputPin(i);
                    pin.On();
                    pin.Off();
                }

                //var lcd4Pin = gpio.NewOutputPin(2, PinBase.LCD_D2);
                //lcd4Pin.On();
                //lcd4Pin.Off();

                var lcd4Pin = gpio.NewOutputPin(3, PinBase.CSIPCK);
                lcd4Pin.On();
                lcd4Pin.Off();
            }
        }

        private static void TestInput()
        {
            using (var gpio = new Gpio(new SshGpioInterface("192.168.0.109")))
            {
                Console.WriteLine("Connected");
                for (short i = 0; i < 8; i++)
                {
                    var pin = gpio.NewInputPin(i);
                    var i1 = i;
                    pin.OnValueChanged += x => Console.WriteLine($"Pin {i1}: Changed to " + x);
                }

                while (true)
                {
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
