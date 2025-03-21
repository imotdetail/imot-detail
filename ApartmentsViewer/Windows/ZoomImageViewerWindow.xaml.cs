using ApartmentsViewer.Models;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ApartmentsViewer.Windows
{ 
    public partial class ZoomImageViewerWindow : Window
    {
		private int currentImageIndex = 0;

		public static readonly DependencyProperty ApartmentProperty =
			DependencyProperty.Register("Apartment", typeof(Apartment), typeof(ZoomImageViewerWindow), new PropertyMetadata(null));

		public ZoomImageViewerWindow(Apartment apartment)
		{
			InitializeComponent();

			this.Apartment = apartment;
			this.UpdateSource();
		}

		public Apartment Apartment
		{
			get { return (Apartment)GetValue(ApartmentProperty); }
			set { SetValue(ApartmentProperty, value); }
		}

		private void PreviousButton_Click(object sender, RoutedEventArgs e)
		{
			this.currentImageIndex--;
			this.UpdateSource();
		}

		private void NextButton_Click(object sender, RoutedEventArgs e)
		{
			this.currentImageIndex++;
			this.UpdateSource();
		}

		private void UpdateSource()
		{
			var bitmap = new BitmapImage();
			bitmap.BeginInit();
			bitmap.UriSource = new Uri(this.Apartment.ImagePaths[this.currentImageIndex], UriKind.RelativeOrAbsolute);
			bitmap.CacheOption = BitmapCacheOption.OnLoad;
			bitmap.EndInit();

			this.SlideshowImage.Source = bitmap;

			this.UpdateButtonsIsEnabledState();
		}

		private void UpdateButtonsIsEnabledState()
		{
			this.previousButton.IsEnabled = this.currentImageIndex > 0;
			this.nextButton.IsEnabled = this.currentImageIndex < this.Apartment.ImagePaths.Count - 1;
		}
	}
}
