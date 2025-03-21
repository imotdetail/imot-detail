namespace ApartmentsViewer.Scrapers
{
    internal static class ScraperFactory
    {
		internal static ScraperBase GetScraper(string url)
        {
            if (url.Contains("imot.bg"))
            {
				return new ImotBgScraper();
			}

            if (url.Contains("homes.bg"))
            {
                return new HomesBgScraper();
            }

            if (url.Contains("dskhome"))
            {
                return new DskHomeScraper();
			}

            throw new NotSupportedException($"Сайтът {url} не се поддържа!");
        }
    }
}
