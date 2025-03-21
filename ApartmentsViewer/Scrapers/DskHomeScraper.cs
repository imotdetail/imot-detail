using ApartmentsViewer.Helpers;
using HtmlAgilityPack;

namespace ApartmentsViewer.Scrapers
{
	internal class DskHomeScraper : ScraperBase
	{
		protected override string ExtractBuildingType(HtmlDocument htmlDocument)
		{
			var buildingTypeDiv = htmlDocument.DocumentNode.SelectNodes("//div").Where(i => i.GetAttributes().Where(a => a.Name == "aria-label").Any(a => a.Value == "Вид строителство")).FirstOrDefault();
			return buildingTypeDiv?.InnerText;
		}

		protected override int? ExtractBuildingYear(HtmlDocument htmlDocument)
		{
			return null;
		}

		protected override string ExtractDescription(HtmlDocument htmlDocument)
		{
			var descriptionParagraph = htmlDocument.DocumentNode.SelectSingleNode("//dsk-read-more-less-text-content//p[@class='ng-star-inserted']");
			if (descriptionParagraph != null)
			{
				return descriptionParagraph.InnerText;
			}
			return null;
		}

		protected override int? ExtractFloor(HtmlDocument htmlDocument)
		{
			var floorDiv = htmlDocument.DocumentNode.SelectNodes("//div[@class='icon-list__item']").Where(i =>
			i.SelectSingleNode(".//mat-icon[@svgicon='otp-floor']") != null).FirstOrDefault();
			var floorSpan = floorDiv?.SelectSingleNode(".//span[@class='icon-list__item__label']");
			if (floorSpan?.InnerText != null)
			{
				var parts = floorSpan.InnerText.Split('/');
				if (parts.Length > 0 && int.TryParse(parts[0], out int floor))
				{
					return floor;
				}
			}
			return null;
		}

		protected override List<string> ExtractImageUrls(HtmlDocument htmlDocument)
		{
			var imageUrls = new List<string>();
			var galleryItems = htmlDocument.DocumentNode.SelectNodes("//gallery-item");

			if (galleryItems != null)
			{
				foreach (var galleryItem in galleryItems)
				{
					var imageTag = galleryItem.SelectSingleNode(".//gallery-image/img[@src]");
					if (imageTag != null && !string.IsNullOrEmpty(imageTag.GetAttributeValue("src", "")))
					{
						imageUrls.Add(imageTag.GetAttributeValue("src", ""));
					}

					var backgroundImageTag = galleryItem.SelectSingleNode(".//div[@class='g-template g-item-template ng-star-inserted']/img[@src]");
					if (imageTag == null && backgroundImageTag != null && !string.IsNullOrEmpty(backgroundImageTag.GetAttributeValue("src", "")))
					{
						imageUrls.Add(backgroundImageTag.GetAttributeValue("src", ""));
					}
				}
			}

			return imageUrls;
		}

		protected override string ExtractNeighborhood(HtmlDocument htmlDocument)
		{
			var locationSpan = htmlDocument.DocumentNode.SelectSingleNode("//span[@class='property-location ng-star-inserted']");
			if (locationSpan != null && !string.IsNullOrEmpty(locationSpan.InnerText))
			{
				var parts = locationSpan.InnerText.Split(',');
				if (parts.Length > 1)
				{
					return parts[1].Trim();
				}

				return locationSpan.InnerText.Trim();
			}
			return null;
		}

		protected override int? ExtractPrice(HtmlDocument htmlDocument)
		{
			var priceTag = HtmlHelper.FindHtmlTagByClass(htmlDocument.DocumentNode, "span", "listing-prices__total-value");
			var priceString = new string(priceTag.InnerText.Where(c => char.IsDigit(c)).ToArray());

			return int.TryParse(priceString, out int price) ? price : null;
		}

		protected override double? ExtractSqMeters(HtmlDocument htmlDocument)
		{
			var sqM = htmlDocument.DocumentNode.SelectNodes("//span[@class='icon-list__item__label']").FirstOrDefault();
			if (sqM != null && int.TryParse(sqM.InnerText, out int sqMeters))
			{
				return sqMeters;
			}
			return null;
		}

		protected override int? ExtractTotalFloors(HtmlDocument htmlDocument)
		{
			var floorDiv = htmlDocument.DocumentNode.SelectNodes("//div[@class='icon-list__item']").Where(i =>
			i.SelectSingleNode(".//mat-icon[@svgicon='otp-floor']") != null).FirstOrDefault();
			var floorSpan = floorDiv?.SelectSingleNode(".//span[@class='icon-list__item__label']");
			if (floorSpan?.InnerText != null)
			{
				var parts = floorSpan.InnerText.Split('/');
				if (parts.Length > 0 && int.TryParse(parts[1], out int floor))
				{
					return floor;
				}
			}
			return null;
		}
	}
}