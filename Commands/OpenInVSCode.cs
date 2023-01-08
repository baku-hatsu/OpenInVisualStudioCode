using System.Diagnostics;
using System.IO;
using System.Linq;

namespace OpenInVisualStudioCode
{
	[Command(PackageIds.OpenInVSCode)]
	internal sealed class OpenInVSCode : BaseCommand<OpenInVSCode>
	{
		private const string VSCodeCommandName = "code";

		protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
		{
			var item = await VS.Solutions.GetActiveItemAsync();

			if (item is null)
			{
				await VS.MessageBox.ShowWarningAsync("OpenInVisualStudioCode", "No items are selected.");
				return;
			}

			if (!CheckIfVSCodeExistsInPath())
			{
				await VS.MessageBox.ShowWarningAsync("OpenInVisualStudioCode", "VS Code is not installed or PATH is not configured.");
				return;
			}

			using var process = new Process();
			process.StartInfo.UseShellExecute = true;
			process.StartInfo.FileName = VSCodeCommandName;
			process.StartInfo.Arguments = item.FullPath;
			process.StartInfo.CreateNoWindow = true;
			process.Start();
		}

		private bool CheckIfVSCodeExistsInPath()
		{
			var dirs = Environment.GetEnvironmentVariable("PATH").Split(';');

			foreach (var dir in dirs)
			{
				if (!dir.Contains("%") && Directory.Exists(dir) && Directory.GetFiles(dir).Any(d => d.Contains("\\" + VSCodeCommandName)))
				{
					return true;
				}
			}

			return false;
		}
	}
}