using System.Text.RegularExpressions;
using System.Text;
using System.Net;

public static class HttpClientHelper
{
    public static HttpClient Client { get; }

    static HttpClientHelper()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        var handler = new HttpClientHandler  
        {  
            // 忽略证书错误  
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true,  
            // 自动解压缩  
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli  
        };
        Client = new HttpClient(handler);
        // 设置请求头
        Client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
            "AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.149 Safari/537.36");
        Client.DefaultRequestHeaders.Add("Accept", "*/*");
        // gzip 会导致乱码
        Client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
        Client.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9");
        Client.DefaultRequestHeaders.Add("Connection", "keep-alive");
    }

    /// <summary>
    /// 下载网页内容，并将其他编码转换为 UTF-8 编码
    /// </summary>
    public static async Task<string> GetWebHtmlAsync(string url)
    {
        var response = await Client.GetAsync(url);
        var bytes = await response.Content.ReadAsByteArrayAsync();

        // 获取网页编码 ContentType 可能为空，从网页获取
        var charset = response.Content.Headers.ContentType?.CharSet;
        if (string.IsNullOrEmpty(charset))
        {
            // 从网页获取编码信息
            var htmldoc = Encoding.UTF8.GetString(bytes);
            var match = Regex.Match(htmldoc, "<meta.*?charset=\"?(?<charset>.*?)\".*?>", RegexOptions.IgnoreCase);
            charset = match.Success ? match.Groups["charset"].Value : "utf-8";
        }

        Encoding encoding = charset.ToLower() switch  
        {  
            "gbk" => Encoding.GetEncoding("GBK"),  
            "gb2312" => Encoding.GetEncoding("GB2312"),  
            "iso-8859-1" => Encoding.GetEncoding("ISO-8859-1"),  
            "ascii" => Encoding.ASCII,  
            "unicode" => Encoding.Unicode,  
            "utf-32" => Encoding.UTF32,  
            _ => Encoding.UTF8  
        };

        // 统一转换为 UTF-8 编码
        var htmlResult = Encoding.UTF8.GetString(Encoding.Convert(encoding, Encoding.UTF8, bytes));
        return htmlResult;
    }
}