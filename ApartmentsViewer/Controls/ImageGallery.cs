using ApartmentsViewer.ViewModels;
using ApartmentsViewer.Windows;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ApartmentsViewer.Controls
{
	public class ImageGallery : Control
	{
		public static readonly DependencyProperty ImagePathsProperty =
			DependencyProperty.Register("ImagePaths", typeof(ObservableCollection<string>), typeof(ImageGallery), new PropertyMetadata(OnImagePathsChanged));

		public static readonly DependencyProperty ImageWidthProperty =
			DependencyProperty.Register("ImageWidth", typeof(double), typeof(ImageGallery), new PropertyMetadata(150.0));

		public static readonly DependencyProperty ImageHeightProperty =
			DependencyProperty.Register("ImageHeight", typeof(double), typeof(ImageGallery), new PropertyMetadata(150.0));
		
		public static readonly DependencyProperty IsPreviousButtonEnabledProperty =
			DependencyProperty.Register("IsPreviousButtonEnabled", typeof(bool), typeof(ImageGallery), new PropertyMetadata(false));

		public static readonly DependencyProperty IsNextButtonEnabledProperty =
			DependencyProperty.Register("IsNextButtonEnabled", typeof(bool), typeof(ImageGallery), new PropertyMetadata(false));

		public static readonly DependencyProperty ButtonsVisibilityProperty =
			DependencyProperty.Register("ButtonsVisibility", typeof(Visibility), typeof(ImageGallery), new PropertyMetadata(Visibility.Collapsed));

		public static readonly DependencyProperty EmptyTextVisibilityProperty =
			DependencyProperty.Register("EmptyTextVisibility", typeof(Visibility), typeof(ImageGallery), new PropertyMetadata(Visibility.Visible));

		private ScrollViewer scrollViewer;

		static ImageGallery()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageGallery), new FrameworkPropertyMetadata(typeof(ImageGallery)));
		}
		
		public ObservableCollection<string> ImagePaths
		{
			get { return (ObservableCollection<string>)GetValue(ImagePathsProperty); }
			set { SetValue(ImagePathsProperty, value); }
		}

		public double ImageWidth
		{
			get { return (double)GetValue(ImageWidthProperty); }
			set { SetValue(ImageWidthProperty, value); }
		}

		public double ImageHeight
		{
			get { return (double)GetValue(ImageHeightProperty); }
			set { SetValue(ImageHeightProperty, value); }
		}

		public bool IsPreviousButtonEnabled
		{
			get { return (bool)GetValue(IsPreviousButtonEnabledProperty); }
			set { SetValue(IsPreviousButtonEnabledProperty, value); }
		}

		public bool IsNextButtonEnabled
		{
			get { return (bool)GetValue(IsNextButtonEnabledProperty); }
			set { SetValue(IsNextButtonEnabledProperty, value); }
		}

		public Visibility ButtonsVisibility
		{
			get { return (Visibility)GetValue(ButtonsVisibilityProperty); }
			set { SetValue(ButtonsVisibilityProperty, value); }
		}

		public Visibility EmptyTextVisibility
		{
			get { return (Visibility)GetValue(EmptyTextVisibilityProperty); }
			set { SetValue(EmptyTextVisibilityProperty, value); }
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			Button prevButton = this.GetTemplateChild("PART_PrevButton") as Button;
			Button nextButton = this.GetTemplateChild("PART_NextButton") as Button;
			this.scrollViewer = this.GetTemplateChild("PART_ImageScrollViewer") as ScrollViewer;

			if (prevButton != null)
			{
				prevButton.Click += this.PrevButton_Click;
			}
			if (nextButton != null)
			{
				nextButton.Click += this.NextButton_Click;
			}

			this.scrollViewer.ScrollChanged += (s,e) => this.UpdateState();
		}

		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			base.OnMouseUp(e);

			if (e.OriginalSource is Image clickedImage)
			{
				var mainViewModel = this.DataContext as MainViewModel;
				ZoomImageViewerWindow zoomWindow = new ZoomImageViewerWindow(mainViewModel.SelectedApartment);
				zoomWindow.ShowDialog();
			}
		}

		private void PrevButton_Click(object sender, RoutedEventArgs e)
		{
			if (this.scrollViewer != null)
			{
				this.scrollViewer.ScrollToHorizontalOffset(Math.Max(0, this.scrollViewer.HorizontalOffset - this.scrollViewer.ViewportWidth));
			}
		}

		private void NextButton_Click(object sender, RoutedEventArgs e)
		{
			if (this.scrollViewer != null)
			{
				this.scrollViewer.ScrollToHorizontalOffset(Math.Min(this.scrollViewer.ExtentWidth, this.scrollViewer.HorizontalOffset + this.scrollViewer.ViewportWidth));
			}
		}

		private void UpdateState()
		{
			if (this.scrollViewer != null)
			{
				this.IsNextButtonEnabled = this.scrollViewer.HorizontalOffset < this.scrollViewer.ScrollableWidth;
				this.IsPreviousButtonEnabled = this.scrollViewer.HorizontalOffset > 0;
			}

			this.ButtonsVisibility = this.ImagePaths != null && this.ImagePaths.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
			this.EmptyTextVisibility = this.ImagePaths == null ? Visibility.Visible : Visibility.Collapsed;
		}

		private static void OnImagePathsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var imageGallery = d as ImageGallery;

			if (imageGallery.scrollViewer != null)
			{
				imageGallery.scrollViewer.ScrollToHorizontalOffset(0);
			}

			imageGallery.UpdateState();
		}
	}
}
