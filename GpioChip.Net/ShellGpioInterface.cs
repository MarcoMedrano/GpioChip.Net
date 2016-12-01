namespace GpioChip.Net
{
    using System.Diagnostics;

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

        public new void Dispose()
        {
            base.Dispose();
        }
    }
}
