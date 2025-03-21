using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace ApartmentsViewer.Models
{
	public class Apartment
	{
		public Apartment()
		{
			ImagePaths = new ObservableCollection<string>();
		}

		public string Description { get; set; }

		public double? SqMeters { get; set; }

		public string Neighborhood{ get; set; }

		public int? Price { get; set; }

		public string Url { get; set; }

		public int? Floor { get; set; }

		public int? TotalFloors { get; set; }

		public string BuildingType{ get; set; }

		public int? BuildingYear { get; set; }

		public string Notes { get; set; }

		public double PricePerSqm
		{
			get
			{
				return Price.HasValue && SqMeters.HasValue && SqMeters.Value != 0 ? Math.Round(Price.Value / SqMeters.Value, 2) : 0;
			}
		}

		public ObservableCollection<string> ImagePaths { get; set; }

		[XmlIgnore]
		public string Link => "Линк";

		[XmlIgnore]
		public List<string> ImageUrls { get; set; }

		public override string ToString()
		{
			return $"{Neighborhood}, Цена: {Price}, {SqMeters} кв.м";
		}
	}
}
