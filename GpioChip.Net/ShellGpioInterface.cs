namespace GpioChip.Net
{
    using System.Diagnostics;

    public class ShellGpioInterface : AbstractShellGpioInterface
    {
        private readonly string username;

        public ShellGpioInterface(string username = "root", string password = "chip") : base()
        {
            this.username = username;
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
            return new AbstractShellCommandResult {};
        }

        public new void Dispose()
        {
            base.Dispose();
        }
    }
}
