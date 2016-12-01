namespace GpioChip.Net
{
    using System;
    using System.Diagnostics;
    using System.IO;

    public class ShellGpioInterface : AbstractShellGpioInterface
    {
        // This parameters are not used but MONO 3.5 is complaining about this class does not have a constructor :S
        //Missing method GpioChip.Net.ShellGpioInterface::.ctor()
        public ShellGpioInterface(string username = "root", string password = "chip") : base()
        {
        }

        protected override AbstractShellCommandResult RunCommand(string command)
        {
            var p = new Process();
            p.StartInfo = new ProcessStartInfo("sh", $"-c '{command}'");
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();

            string res = p.StandardOutput.ReadToEnd();
            while (!p.HasExited)
            {
                res += p.StandardOutput.ReadToEnd();
            }

            return new AbstractShellCommandResult { Result = res, ExitCode = p.ExitCode };
        }

        public override void Init(short pin)
        {
            base.Init(pin);
            Console.WriteLine("Initializing pin " + pin);
            var watcher = new FileSystemWatcher($"/sys/class/gpio/gpio{pin}", "value");
            //watcher.NotifyFilter = NotifyFilters.Size;

            watcher.Error += this.OnError;
            watcher.Changed += (s, e) => { Console.WriteLine("Changed " + pin); this.OnChanged(pin, s, e); };
            watcher.EnableRaisingEvents = true;
        }

        private void OnChanged(short pin, object sender, FileSystemEventArgs e)
        {
            if(e.ChangeType != WatcherChangeTypes.Changed) return;
            if (this.pinesSubscribed[pin].AreEventsSubscribed == false) return;

            var newValue = this.GetValue(pin);

            if (newValue != this.pinesSubscribed[pin].LastValue)
            {
                this.pinesSubscribed[pin].LastValue = newValue;
                this.pinesSubscribed[pin].RaiseEvent(newValue);
            }
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            Console.WriteLine(e.GetException());
        }

        public new void Dispose()
        {
            base.Dispose();
        }
    }
}
