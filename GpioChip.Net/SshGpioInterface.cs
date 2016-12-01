namespace GpioChip.Net
{
    using Renci.SshNet;

    public class SshGpioInterface : AbstractShellGpioInterface
    {
        private readonly SshClient client;

        public SshGpioInterface(string host, string username = "root", string password = "chip") : base()
        {
            this.client = new SshClient(host, username, password);
            this.client.Connect();

        }

        protected override AbstractShellCommandResult RunCommand(string command)
        {
            var res = this.client.RunCommand(command);
            return new AbstractShellCommandResult {Result = res.Result, ExitCode = res.ExitStatus, ErrorMessage = res.Error };
        }

        public new void Dispose()
        {
            base.Dispose();

            this.client.Disconnect();
            this.client.Dispose();
        }
    }
}
