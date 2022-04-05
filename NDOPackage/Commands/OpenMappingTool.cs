namespace NDOVsPackage.Commands
{
    [Command(PackageGuids.guidNDOPackageCmdSetString, PackageIds.cmdidOpenMappingTool)]
    internal sealed class OpenMappingTool : BaseCommand<OpenMappingTool>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            await VS.MessageBox.ShowWarningAsync("OpenMappingTool", "Button clicked");
        }
    }
}
