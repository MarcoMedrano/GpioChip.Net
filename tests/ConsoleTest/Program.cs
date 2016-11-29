using System;

namespace GpioChip.Net.tests.ConsoleTest
{
    using System.Security.Cryptography.X509Certificates;
    using System.Threading;

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Running GPIO");
            TestOutput();
            //TestInput();
            Console.WriteLine("Finished");
            Console.ReadKey();
        }

        private static void TestOutput()
        {
            using (var gpio = new Gpio(new SshGpioInterface("192.168.0.109")))
            {
                Console.WriteLine("Connected");
                for (short i = 0; i < 8; i++)
                {
                    var pin = gpio.NewOutputPin(i);
                    pin.On();
                    pin.Off();
                }
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
