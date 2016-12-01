namespace GpioChip.Net
{
    using System.Collections.Generic;
    using System.Threading;

    using Renci.SshNet;

    public class SshGpioInterface : AbstractShellGpioInterface
    {
        private readonly SshClient client;

        public SshGpioInterface(string host, string username = "root", string password = "chip") : base()
        {
            this.client = new SshClient(host, username, password);
            this.client.Connect();

            new Thread(this.CheckValueChanged).Start();
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

        private void CheckValueChanged()
        {
            while (true)
            {
                lock (this.pinesSubscribed)
                {
                    foreach (KeyValuePair<short, PinSubscription> keyValue in this.pinesSubscribed)
                    {
                        if (this.pinesSubscribed[keyValue.Key].AreEventsSubscribed == false) continue;

                        var newValue = this.GetValue(keyValue.Key);

                        if (newValue != keyValue.Value.LastValue)
                        {
                            keyValue.Value.LastValue = newValue;
                            keyValue.Value.RaiseEvent(newValue);
                        }
                    }
                }
            }
        }
    }
}
