# GpioChip.Net

## Remotely

Thanks to SSH this is fastest way to try GPIO without make a line of code or bash inside CHIP itself:

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
            
This version can run also inside CHIP by using mono like: `mono ConsoleTest.exe`


## Locally

This snipped can run inside CHIP without use SSH which makes it run faster than the _remote_ version although is using sysfs interface. Just change less than one line of code.

           using (var gpio = new Gpio(new ShellGpioInterface()))
            {
                Console.WriteLine("Connected");
                for (short i = 0; i < 8; i++)
                {
                    var pin = gpio.NewOutputPin(i);
                    pin.On();
                    pin.Off();
                }
            }
            

## GPIO Input, Interruptions?

		  var pin = gpio.NewInputPin(0);
		  pin.OnValueChanged += x => Console.WriteLine($"Pin 0: Changed to " + x);
		  Console.ReadKey();


## Expanding GPIO

Use the extra pins if you are not using the LCD

		    var lcd4Pin = gpio.NewOutputPin(2, PinBase.LCD_D2);
		    lcd4Pin.On();
		    lcd4Pin.Off();


### Need help installing MONO on CHIP?**

    apt-get install aptitude
    aptitude install mono-complete
