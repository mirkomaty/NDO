using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Project = EnvDTE.Project;

namespace NDOVsPackage.Commands
{
    [Command(PackageGuids.guidNDOPackageCmdSetString, PackageIds.cmdidAddClass)]
    internal sealed class AddPersistentClass : BaseCommand<AddPersistentClass>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
			await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
			System.Array solObjects = (Array) this.VisualStudioApplication.ActiveSolutionProjects;
			if (solObjects.Length < 1)
				return;

			Project project = (Project) solObjects.GetValue( 0 );

			if (!(CodeGenHelper.IsCsOrVbProject(project)))
				return;

			PersistentClassDialog pcd = new PersistentClassDialog();
			pcd.ShowDialog();
			if (pcd.Result == DialogResult.Cancel)
				return;

			SelectedItems selItems = VisualStudioApplication.SelectedItems;
			ProjectItem parentItem = null;
			if (selItems.Count == 1)
			{
				IEnumerator ienum = selItems.GetEnumerator();
				ienum.MoveNext();
				SelectedItem si = (SelectedItem) ienum.Current;
				if (si.ProjectItem != null && si.ProjectItem.Kind == "{6BB5F8EF-4483-11D3-8BCF-00C04F8EC28C}") // Folder
					parentItem = si.ProjectItem;
			}

			if (CodeGenHelper.IsVbProject( project ))
				new AddPersistentClassVb( project, pcd.ClassName, pcd.Serializable, parentItem ).DoIt();
			else if (CodeGenHelper.IsCsProject( project ))
				new AddPersistentClassCs( project, pcd.ClassName, pcd.Serializable, parentItem ).DoIt();
		}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString() );
				MessageBox.Show(ex.Message, "Configure" );
			}
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
