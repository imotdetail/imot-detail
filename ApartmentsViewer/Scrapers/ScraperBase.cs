using ApartmentsViewer.Models;
using HtmlAgilityPack;
using PuppeteerSharp;
using System.Text;

namespace ApartmentsViewer.Scrapers
{
	internal abstract class ScraperBase
    {
		static ScraperBase()
		{
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
		}

		internal virtual string BaseUrl
		{
			get;
		}

		internal virtual Encoding Encoding => Encoding.UTF8;

		internal async Task<string> LoadHtmlFromUrlAsyncPuppeteer(string url)
		{
			try
			{
				using (var browser = await Puppeteer.LaunchAsync(new LaunchOptions
				{
					Headless = true
				}))

				using (var page = await browser.NewPageAsync())
				{
					await page.GoToAsync(url, new NavigationOptions
					{
						WaitUntil = [WaitUntilNavigation.Load, WaitUntilNavigation.Networkidle2, WaitUntilNavigation.DOMContentLoaded]
					});

					string content = await page.GetContentAsync();
					return content;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error loading dynamic HTML with Puppeteer: {ex.Message}");
				return null;
			}
		}

		protected abstract List<string> ExtractImageUrls(HtmlDocument htmlDocument);
		protected abstract int? ExtractPrice(HtmlDocument htmlDocument);
		protected abstract double? ExtractSqMeters(HtmlDocument htmlDocument);
		protected abstract string ExtractNeighborhood(HtmlDocument htmlDocument);
		protected abstract string ExtractDescription(HtmlDocument htmlDocument);
		protected abstract int? ExtractFloor(HtmlDocument htmlDocument);
		protected abstract int? ExtractTotalFloors(HtmlDocument htmlDocument);
		protected abstract string ExtractBuildingType(HtmlDocument htmlDocument);
		protected abstract int? ExtractBuildingYear(HtmlDocument htmlDocument);

		protected async Task<HtmlDocument> GetHtmlContent(string url)
		{
			HtmlDocument htmlDocument = new HtmlDocument();

			await Task.Run(async () =>
			{
				var htmlContent = await this.LoadHtmlFromUrlAsyncPuppeteer(url);
				htmlDocument.LoadHtml(htmlContent);
			});
			
			return htmlDocument;
		}

		internal async Task<Apartment> ScrapeApartment(string url)
		{
			HtmlDocument htmlDocument = await this.GetHtmlContent(url);
			if (htmlDocument == null)
			{
				return null;
			}

			return new Apartment()
			{
				Url = url,
				ImageUrls = ExtractImageUrls(htmlDocument),
				Price = ExtractPrice(htmlDocument),
				SqMeters = ExtractSqMeters(htmlDocument),
				Neighborhood = ExtractNeighborhood(htmlDocument),
				Description = ExtractDescription(htmlDocument),
				Floor = ExtractFloor(htmlDocument),
				TotalFloors = ExtractTotalFloors(htmlDocument),
				BuildingType = ExtractBuildingType(htmlDocument),
				BuildingYear = ExtractBuildingYear(htmlDocument)
			};
		}
	}
}
