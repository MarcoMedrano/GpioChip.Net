namespace GpioChip.Net
{
    using System;
    using System.Diagnostics;

    public class ShellGpioInterface : AbstractShellGpioInterface
    {
        Process currentProcess = Process.GetCurrentProcess();

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

            var result = new AbstractShellCommandResult { Result = res, ExitCode = p.ExitCode };
            p = null;
            this.FreeMemory();

            return result;
        }

        /// <summary>
        /// Due MONO on CHIP seems not freeing enough memory, I will had to free some space.
        /// </summary>
        private void FreeMemory()
        {
            //if more than 10MB
            if ((this.currentProcess.WorkingSet64 / 1024 / 1024) > 10 )
                GC.Collect();
        }

        public new void Dispose()
        {
            base.Dispose();
        }
    }
}
