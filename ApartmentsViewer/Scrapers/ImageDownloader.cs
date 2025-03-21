using System.IO;
using System.Net.Http;

namespace ApartmentsViewer.Scrapers
{
    internal class ImageDownloader
    {
		internal static async Task<string> DownloadImage(string imageUrl, string outputDirectory, string baseUrl)
		{
			var client = new HttpClient();
			
			try
			{
				Uri uri = new Uri(imageUrl, UriKind.RelativeOrAbsolute);
				if (!uri.IsAbsoluteUri)
				{
					if (!string.IsNullOrEmpty(baseUrl))
					{
						try
						{
							uri = new Uri(new Uri(baseUrl), imageUrl);
						}
						catch (UriFormatException uriEx)
						{
							return null;
						}
					}
					else
					{
						return null;
					}
				}

				HttpResponseMessage response = await client.GetAsync(uri);
				response.EnsureSuccessStatusCode();

				byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();

				string fileExtension = Path.GetExtension(imageUrl);
				string uniqueFileName = Path.Combine(outputDirectory,Guid.NewGuid().ToString() + fileExtension);

				await File.WriteAllBytesAsync(uniqueFileName, imageBytes);

				return uniqueFileName;
			}
			catch (Exception e)
			{
			}
			finally
			{
				client.Dispose();
			}

			return null;
		}
	}
}
