using PuppeteerSharp;
using System.Windows;

namespace ApartmentsViewer;


public partial class App : Application
{
	static App()
	{
		Task.Run(async () =>
		{
			await new BrowserFetcher().DownloadAsync();
		});
	}
}

