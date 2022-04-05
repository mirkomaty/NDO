namespace NDOVsPackage.Commands
{
    [Command(PackageGuids.guidNDOPackageCmdSetString, PackageIds.cmdidNDOConfiguration)]
    internal sealed class Configure : BaseCommand<Configure>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            await VS.MessageBox.ShowWarningAsync("Configure", "Button clicked");
        }

        protected override void BeforeQueryStatus(EventArgs e)
        {
            Command.Visible = true;
        }
    }
}
