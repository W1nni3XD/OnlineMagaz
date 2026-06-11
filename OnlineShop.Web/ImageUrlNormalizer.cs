namespace OnlineShop.Web;

public static class ImageUrlNormalizer
{
    public static string? Normalize(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return null;

        url = url.Trim();
        if (url.StartsWith('/'))
            return url;

        if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
            return uri.AbsolutePath;

        return url;
    }
}
