namespace NDOVsPackage.Commands
{
    [Command(PackageGuids.guidNDOPackageCmdSetString, PackageIds.cmdidAddRelation)]
    internal sealed class AddRelation : BaseCommand<AddRelation>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            await VS.MessageBox.ShowWarningAsync("AddRelation", "Button clicked");
        }

        protected override void BeforeQueryStatus(EventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                var active = await VS.Documents.GetActiveDocumentViewAsync();
                var enabled = active.FilePath.EndsWith(".cs") || active.FilePath.EndsWith(".vb");
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                Command.Enabled = enabled;
            });
        }
    }
}
