using HtmlAgilityPack;

namespace ApartmentsViewer.Helpers
{
	internal static class HtmlHelper
	{
		internal static HtmlNode FindHtmlTagByContent(HtmlNode documentNode, string tagName, string searchText, int skip = 0)
		{
			return FindNodes(documentNode, $"//{tagName}", node =>
				node.InnerText.Contains(searchText, StringComparison.OrdinalIgnoreCase), skip);
		}

		internal static HtmlNode FindHtmlTagByClass(HtmlNode documentNode, string tagName, string className, int skip = 0)
		{
			return FindNodes(documentNode, $"//{tagName}", node => node.HasClass(className), skip);
		}

		internal static HtmlNode FindHtmlTagByID(HtmlNode documentNode, string id)
		{
			return documentNode.SelectSingleNode($"//*[@id='{id}']");
		}

		internal static List<string> FindImageUrls(HtmlNode documentNode)
		{
			return documentNode.SelectNodes("//img")?
			.Select(imgNode => imgNode.GetAttributeValue("src", null))
			.Where(src => !string.IsNullOrEmpty(src))
			.ToList() ?? new List<string>();
		}

		internal static HtmlNode FindNodes(HtmlNode documentNode, string xpath, Func<HtmlNode, bool> predicate, int skip)
		{
			return documentNode.SelectNodes(xpath)?
		   .Where(predicate)
		   .Skip(skip)
		   .FirstOrDefault();
		}
	}
}
