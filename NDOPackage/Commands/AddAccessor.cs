using System.Diagnostics;
using EnvDTE;
using MessageBox = System.Windows.Forms.MessageBox;

namespace NDOVsPackage.Commands
{
    [Command(PackageGuids.guidNDOPackageCmdSetString, PackageIds.cmdidAddAccessor)]
    internal sealed class AddAccessor : BaseCommand<AddAccessor>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
			try
			{
				Document document;
				TextDocument textDoc;

				await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

				document = ApplicationObject.VisualStudioApplication.ActiveDocument;
				if (document == null)
					return;

				textDoc = (TextDocument) document.Object( "TextDocument" );
				if (textDoc == null)
					return;

				string fileName = document.FullName.ToLower();
				if (fileName.EndsWith( ".cs" ))
					new AddAccessorCs( textDoc, document ).DoIt();
				else if (fileName.EndsWith( ".vb" ))
					new AddAccessorVb( textDoc, document ).DoIt();
			}
			catch (Exception ex)
			{
				Debug.WriteLine( ex.ToString() );
				MessageBox.Show( ex.Message, "Configure" );
			}
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
