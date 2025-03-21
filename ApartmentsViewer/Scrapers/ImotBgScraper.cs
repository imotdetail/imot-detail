using ApartmentsViewer.Helpers;
using ApartmentsViewer.Models;
using HtmlAgilityPack;
using System.Text;
using System.Windows.Media;

namespace ApartmentsViewer.Scrapers
{
	internal class ImotBgScraper : ScraperBase
	{
		internal const string ImotBgBaseUrl = "https://www.imot.bg/";

		internal override string BaseUrl
		{
			get
			{
				return ImotBgBaseUrl;
			}
		}

		internal override Encoding Encoding => Encoding.GetEncoding("windows-1251");

		protected override string ExtractBuildingType(HtmlDocument htmlDocument)
		{
			var buildintTypeText = HtmlHelper.FindHtmlTagByContent(htmlDocument.DocumentNode, "div", "Строителство:", 2)?.InnerText?.Split(" ");
			var buildingType = buildintTypeText?[1]?.Trim(',');

			return buildingType;
		}

		protected override int? ExtractBuildingYear(HtmlDocument htmlDocument)
		{
			var buildingTypeText = HtmlHelper.FindHtmlTagByContent(htmlDocument.DocumentNode, "div", "Строителство:", 2)?.InnerText?.Split(" ");
			var buildingYearString = buildingTypeText.Length >= 3 ? buildingTypeText?[2] : null;

			return int.TryParse(buildingYearString, out int buildingYear) ? buildingYear : null;
		}

		protected override string ExtractDescription(HtmlDocument htmlDocument)
		{
			var description = HtmlHelper.FindHtmlTagByID(htmlDocument.DocumentNode, "description_div")?.InnerText;

			return description;
		}

		protected override int? ExtractFloor(HtmlDocument htmlDocument)
		{
			var floors = HtmlHelper.FindHtmlTagByContent(htmlDocument.DocumentNode, "div", "Етаж:", 6).InnerText;
			var floorNumberText = floors.ToList().FirstOrDefault(char.IsDigit);

			return int.TryParse(floorNumberText.ToString(), out int floorNumber) ? floorNumber : null;
		}

		protected override List<string> ExtractImageUrls(HtmlDocument htmlDocument)
		{
			var imgs = htmlDocument.DocumentNode.SelectNodes("//img").Where(i => i.GetAttributes().Any(a => a.Name == "data-src-gallery"));
			var imageUrls = imgs.SelectMany(i => i.GetAttributes().Where(a => a.Name == "data-src-gallery").Select(a => a.Value)).ToList();

			return imageUrls;
		}

		protected override string ExtractNeighborhood(HtmlDocument htmlDocument)
		{
			var neighborHood = HtmlHelper.FindHtmlTagByClass(htmlDocument.DocumentNode, "div", "location");
			var neighborHoodStr = neighborHood?.InnerText?.Split(",")?[1]?.Trim();

			return neighborHoodStr;
		}

		protected override int? ExtractPrice(HtmlDocument htmlDocument)
		{
			var priceTag = HtmlHelper.FindHtmlTagByID(htmlDocument.DocumentNode, "cena");
			var strPrice = priceTag?.InnerText?.Substring(1, priceTag.InnerText.IndexOf("EUR") - 2)?.Replace(" ", "");

			return int.TryParse(strPrice, out int price) ? price : null;
		}

		protected override double? ExtractSqMeters(HtmlDocument htmlDocument)
		{
			var sqM = HtmlHelper.FindHtmlTagByContent(htmlDocument.DocumentNode, "div", "Площ:");
			var sqMText = HtmlHelper.FindHtmlTagByContent(sqM, "strong", "m").InnerText;
			var strSqm = sqMText.Substring(0, sqMText.Length - " m2".Length).Trim();

			return double.TryParse(strSqm, out double sqMeters) ? sqMeters : null;
		}

		protected override int? ExtractTotalFloors(HtmlDocument htmlDocument)
		{
			var floors = HtmlHelper.FindHtmlTagByContent(htmlDocument.DocumentNode, "div", "Етаж:", 6)?.InnerText;
			var totalFloorsText = floors?.ToList()?.LastOrDefault(char.IsDigit);

			return int.TryParse(totalFloorsText.ToString(), out int totalFloors) ? totalFloors : null;
		}
	}
}
