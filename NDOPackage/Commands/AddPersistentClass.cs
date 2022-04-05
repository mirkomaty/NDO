using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace NDOVsPackage.Commands
{
    [Command(PackageGuids.guidNDOPackageCmdSetString, PackageIds.cmdidAddClass)]
    internal sealed class AddPersistentClass : BaseCommand<AddPersistentClass>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            await VS.MessageBox.ShowWarningAsync("AddPersistentClass", "Button clicked");
        }

        protected override void BeforeQueryStatus(EventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                var project = await VS.Solutions.GetActiveProjectAsync();
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                var enabled = CodeGenHelper.IsCsOrVbProject(project);
                Command.Enabled = enabled;
            });
        }
    }
}
