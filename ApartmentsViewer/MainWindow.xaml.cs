using ApartmentsViewer.ViewModels;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ApartmentsViewer;

public partial class MainWindow : Window
{
	private MainViewModel _viewModel;

	public MainWindow()
	{
		InitializeComponent();
		_viewModel = (MainViewModel)DataContext;
	}

	protected override void OnClosing(CancelEventArgs e)
	{
		base.OnClosing(e);
		_viewModel.SaveApartments();
	}

	private void DG_Hyperlink_Click(object sender, RoutedEventArgs e)
	{
		Hyperlink link = (Hyperlink)e.OriginalSource;

		ProcessStartInfo startInfo = new ProcessStartInfo(link.NavigateUri.AbsoluteUri)
		{
			UseShellExecute = true
		};

		Process.Start(startInfo);
	}

	private void OnCheckBoxChecked(object sender, RoutedEventArgs e)
	{
		var checkBox = sender as CheckBox;
		
		if (this.ApartmentsDataGrid != null && checkBox != null && checkBox.IsChecked.HasValue)
		{
			var columnIndex = int.Parse(checkBox.Tag.ToString());
			this.ApartmentsDataGrid.Columns[columnIndex].Visibility = checkBox.IsChecked.Value ? Visibility.Visible : Visibility.Collapsed;
		}
	}
}
