using System.Diagnostics;

namespace NDOVsPackage.Commands
{
    [Command(PackageGuids.guidNDOPackageCmdSetString, PackageIds.cmdidAddAccessor)]
    internal sealed class AddAccessor : BaseCommand<AddAccessor>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            await VS.MessageBox.ShowWarningAsync("NDOPackage", "Button clicked");
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
