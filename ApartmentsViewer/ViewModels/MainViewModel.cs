using ApartmentsViewer.Helpers;
using ApartmentsViewer.Models;
using ApartmentsViewer.Scrapers;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml.Serialization;

namespace ApartmentsViewer.ViewModels
{
	public class MainViewModel : ViewModelBase
	{
		private string apartmentImagesDirectory = Path.Combine(Path.GetTempPath(), "ApartmentsViewer_Images");
		private Apartment selectedApartment;
		private bool isBusy;
		private ObservableCollection<Apartment> apartments = new ObservableCollection<Apartment>();
		private readonly string DataFileName = Path.Combine(Path.GetTempPath(), "ApartmentsViewerApartments.xml");

		private string url;
		private ICollectionView apartmentView;

		public MainViewModel()
		{
			if (!Directory.Exists(apartmentImagesDirectory))
			{
				Directory.CreateDirectory(apartmentImagesDirectory);
			}

			this.LoadApartments();

			this.apartmentView = CollectionViewSource.GetDefaultView(this.Apartments);
			this.apartmentView.Filter = this.OnApartmentsFilter;

			this.AddApartmentCommand = new RelayCommand((_) => AddApartment());
			this.RemoveApartmentCommand = new RelayCommand(RemoveApartment);
			this.SearchCommand = new RelayCommand((_) => SearchApartments());
			this.DeleteAllApartmentsCommand = new RelayCommand((_) => DeleteAllApartments());
		}

		public Apartment SelectedApartment
		{
			get { return this.selectedApartment; }
			set
			{
				if (this.selectedApartment != value)
				{
					this.selectedApartment = value;
					this.OnPropertyChanged("SelectedApartment");
				}
			}
		}

		public ObservableCollection<Apartment> Apartments
		{
			get { return this.apartments; }
			set
			{
				if (this.apartments != value)
				{
					this.apartments = value;
					this.OnPropertyChanged(nameof(Apartments));
				}
			}
		}

		public ICollectionView ApartmentView
		{
			get { return this.apartmentView; }
		}

		public bool IsBusy
		{
			get { return this.isBusy; }
			set 
			{ 
				if (this.isBusy != value)
				{
					this.isBusy = value;
					this.OnPropertyChanged(nameof(IsBusy));
				}
			}
		}

		public string Url
		{
			get { return this.url; }
			set 
			{ 
				if (this.url != value)
				{
					this.url = value;
					this.OnPropertyChanged(nameof(Url));
				}
			}
		}

		public bool AreAggregatesVisible
		{
			get
			{
				return this.ApartmentView.Cast<Apartment>().Any();
			}
		}

		public double AveragePrice
		{
			get
			{
				var prices = this.ApartmentView.OfType<Apartment>().Where(a => a.Price != null).Select(a => a.Price.Value);
				return prices.Any() ? Math.Round(prices.Average()) : 0.0;
			}
		}

		public double AverageSqMeters
		{
			get
			{
				var sqMeters = this.ApartmentView.OfType<Apartment>().Where(a => a.SqMeters != null).Select(a => a.SqMeters.Value);
				return sqMeters.Any() ? Math.Round(sqMeters.Average()) : 0.0;
			}
		}

		public double AveragePricePerSqMeters
		{
			get
			{
				var pricePerSqMeters = this.ApartmentView.OfType<Apartment>().Where(a => a.PricePerSqm > 0).Select(a => a.PricePerSqm);
				return pricePerSqMeters.Any() ? Math.Round(pricePerSqMeters.Average(), 2) : 0.0;
			}
		}

		public string SearchText { get; set; }

		public ICommand AddApartmentCommand { get; }

		public ICommand RemoveApartmentCommand { get; }

		public ICommand SearchCommand { get; }

		public ICommand DeleteAllApartmentsCommand { get; }

		private async void DownloadImages(Apartment apartment, string baseUrl, Action onCompleted)
		{
			foreach (var imageUrl in apartment.ImageUrls)
			{
				var image = await ImageDownloader.DownloadImage(imageUrl, apartmentImagesDirectory, baseUrl);

				if (!string.IsNullOrEmpty(image))
				{
					apartment.ImagePaths.Add(image);
				}
			}

			onCompleted();
		}

		private async void AddApartment()
		{
			try
			{
				this.IsBusy = true;

				var scraper = ScraperFactory.GetScraper(this.Url);
				var apartment = await scraper.ScrapeApartment(this.Url);
				this.Apartments.Add(apartment);

				this.DownloadImages(apartment, scraper.BaseUrl, this.SaveApartments);

				this.SelectedApartment = apartment;

				this.RefreshAggregateData();
			}
			catch (Exception e)
			{
				MessageBox.Show("Грешка при добавяне на апартамент!");
			}
			finally
			{
				this.IsBusy = false;
				this.Url = string.Empty;
			}
		}

		private void RemoveApartment(object parameter)
		{
			Apartment apartmentToRemove = parameter as Apartment;
			if (apartmentToRemove != null)
			{
				if (MessageBox.Show($"Сигурни ли сте, че искате да изтриете '{apartmentToRemove}'?", "Потвърди", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
				{
					foreach (var imageUrl in apartmentToRemove.ImagePaths)
					{
						File.Delete(imageUrl);
					}

					Apartments.Remove(apartmentToRemove);

					this.SaveApartments();

					this.RefreshAggregateData();
				}
			}
		}

		private void DeleteAllApartments()
		{
			if (MessageBox.Show("Сигурни ли сте, че искате да изтриете всички апартаменти?", "Потвърди", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
			{
				foreach (var apartment in this.Apartments)
				{
					foreach (var imageUrl in apartment.ImagePaths)
					{
						File.Delete(imageUrl);
					}
				}

				this.Apartments.Clear();
				this.SaveApartments();
				this.RefreshAggregateData();
			}
		}

		private void LoadApartments()
		{
			if (File.Exists(DataFileName))
			{
				XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<Apartment>));
				using (FileStream fileStream = new FileStream(DataFileName, FileMode.Open))
				{
					try
					{
						ObservableCollection<Apartment> loadedApartments = serializer.Deserialize(fileStream) as ObservableCollection<Apartment>;
						if (loadedApartments != null)
						{
							this.Apartments = loadedApartments;
						}
					}
					catch (Exception ex)
					{
						MessageBox.Show($"Грешка при зареждане на арартаменти: {ex.Message}", "Грешка при зареждане", MessageBoxButton.OK, MessageBoxImage.Error);
					}
				}
			}
		}

		internal void SaveApartments()
		{
			XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<Apartment>));
			using (FileStream fileStream = new FileStream(DataFileName, FileMode.Create))
			{
				try
				{
					serializer.Serialize(fileStream, Apartments);
				}
				catch (Exception ex)
				{
					MessageBox.Show($"Грешка при запазване на апартаменти: {ex.Message}", "Грешка при запазване", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		private void SearchApartments()
		{
			this.ApartmentView.Refresh();

			this.RefreshAggregateData();
		}

		private void RefreshAggregateData()
		{
			this.OnPropertyChanged("AreAggregatesVisible");
			this.OnPropertyChanged("AveragePrice");
			this.OnPropertyChanged("AverageSqMeters");
			this.OnPropertyChanged("AveragePricePerSqMeters");
		}

		private bool OnApartmentsFilter(object obj)
		{
			if (string.IsNullOrEmpty(this.SearchText))
			{
				return true;
			}

			var apartment = obj as Apartment;
			var searchText = this.SearchText.ToLower();

			bool? descriptionFilter = apartment.Description?.ToLower()?.Contains(searchText);
			bool? priceFilter = apartment.Price?.ToString()?.ToLower()?.StartsWith(searchText);
			bool? sqMetersFilter = apartment.SqMeters?.ToString()?.ToLower()?.StartsWith(searchText);
			bool? buildingTypeFilter = apartment.BuildingType?.Contains(searchText);
			bool? floorFilter = apartment.Floor?.ToString()?.StartsWith(searchText);
			bool? totalFloorsFilter = apartment.TotalFloors?.ToString()?.StartsWith(searchText);
			bool? buildingYearFilter = apartment.BuildingYear?.ToString()?.StartsWith(searchText);
			bool? neighborhoodFilter = apartment.Neighborhood?.Contains(searchText);

			return (descriptionFilter.HasValue && descriptionFilter.Value) || 
					(priceFilter.HasValue && priceFilter.Value) ||
					(sqMetersFilter.HasValue && sqMetersFilter.Value) ||
					(buildingTypeFilter.HasValue && buildingTypeFilter.Value) ||
					(floorFilter.HasValue && floorFilter.Value) ||
					(totalFloorsFilter.HasValue && totalFloorsFilter.Value) ||
					(buildingYearFilter.HasValue && buildingYearFilter.Value) || 
					(neighborhoodFilter.HasValue && neighborhoodFilter.Value					);
		}
	}
}
