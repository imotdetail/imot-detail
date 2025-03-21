using ApartmentsViewer.Helpers;
using HtmlAgilityPack;

namespace ApartmentsViewer.Scrapers
{
	internal class HomesBgScraper : ScraperBase
	{
		internal const string HomesBgBaseUrl = "https://www.homes.bg/";

		internal override string BaseUrl => HomesBgBaseUrl;

		protected override string ExtractBuildingType(HtmlDocument htmlDocument)
		{
			var buildingTypeTag = HtmlHelper.FindHtmlTagByContent(htmlDocument.DocumentNode, "span", "Строителство");

			return buildingTypeTag?.InnerText.Split(":")?[1]?.Trim();
		}

		protected override int? ExtractBuildingYear(HtmlDocument htmlDocument)
		{
			return null;
		}

		protected override string ExtractDescription(HtmlDocument htmlDocument)
		{
			var description = HtmlHelper.FindHtmlTagByClass(htmlDocument.DocumentNode, "div", "description").InnerText;

			return description;
		}

		protected override int? ExtractFloor(HtmlDocument htmlDocument)
		{
			var floorTag = HtmlHelper.FindHtmlTagByContent(htmlDocument.DocumentNode, "span", "Етаж");
			var parentTag = floorTag.ParentNode;
			var floorNumberTag = HtmlHelper.FindHtmlTagByContent(parentTag, "h3", "-");

			return int.TryParse(floorNumberTag.InnerText[0].ToString(), out int floor) ? floor : null;
		}

		protected override List<string> ExtractImageUrls(HtmlDocument htmlDocument)
		{
			var imgs = HtmlHelper.FindImageUrls(htmlDocument.DocumentNode);

			return imgs.Where(i => i.Contains("g5.homes.bg")).ToList();
		}

		protected override string ExtractNeighborhood(HtmlDocument htmlDocument)
		{
			string tdPath = $"//td";
			var neighBorHood = htmlDocument.DocumentNode.SelectNodes(tdPath).Skip(2).First();
			var neighBorHoodParsed = neighBorHood.InnerText.Split(" ")[1].Trim(',');

			return neighBorHoodParsed;
		}

		protected override int? ExtractPrice(HtmlDocument htmlDocument)
		{
			var price = HtmlHelper.FindHtmlTagByClass(htmlDocument.DocumentNode, "span", "ver20black")?.InnerText?.Trim();
			
			return int.TryParse(price, System.Globalization.NumberStyles.AllowThousands, null, out int priceParsed) ? priceParsed : null;
		}

		protected override double? ExtractSqMeters(HtmlDocument htmlDocument)
		{
			var price = ExtractPrice(htmlDocument);

			var pricePerSqM = HtmlHelper.FindHtmlTagByContent(htmlDocument.DocumentNode, "td", "EUR/m²");
			var strPricePerSqM = pricePerSqM.InnerText.Substring(pricePerSqM.InnerText.IndexOf("(")).Split(" ")[0].Substring(1);
			var pricePerSqMParsed = double.Parse(strPricePerSqM);

			var sqMeters = Math.Round((double)price / pricePerSqMParsed, 2);

			return sqMeters;
		}

		protected override int? ExtractTotalFloors(HtmlDocument htmlDocument)
		{
			var floorTag = HtmlHelper.FindHtmlTagByContent(htmlDocument.DocumentNode, "span", "Етажи");
			var parentTag = floorTag.ParentNode;
			var floorNumberTag = HtmlHelper.FindNodes(parentTag, "//h3", (a) => { return true; }, 2);

			return int.TryParse(floorNumberTag.InnerText[0].ToString(), out int floor) ? floor : null;
		}
	}
}
