using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using HtmlAgilityPack;
using BookMaker.Utils;
using System.Text.RegularExpressions;

namespace BookMaker.BookTool{

    internal static class BookScraper{

        internal static async Task<int> MakeChapter(DownloadOptions opts)
        {
            // 创建目录
            Directory.CreateDirectory(opts.Dir);

            // 判断 json 是否存在
            if (!File.Exists(opts.Json))
            {
                Console.WriteLine($"File {opts.Json} not exists, exiting...");
                return 1;
            }

            var json = File.ReadAllText(opts.Json);
            var chapters = JsonSerializer.Deserialize<List<Chapter>>(json);

            // 遍历 chapters 支持从指定位置开始
            int i = 1;
            foreach (var chapter in chapters)
            {
                if(opts.Begin > 1 && i < opts.Begin)
                {
                    i++;
                    continue;
                }
                Console.WriteLine($"Downloading {i}/{chapters.Count} {chapter.Title} - {chapter.Url}");
                // 判断文件是否存在， 文件名序号补零为了方便排序
                var filePath = $"{opts.Dir}/chapter_{i.ToString().PadLeft(5, '0')}.txt";
                if (File.Exists(filePath))
                {
                    Console.WriteLine($"File {filePath} already exists, skipping...");
                    i++;
                    continue;
                }
                // 下载章节内容
                var content = await DownloadChapter(chapter.Url, opts.Selector, opts.Next, opts.Regex);
                // 不包含章节标题 方便后面 epub 生成
                //content = $"\n{chapter.Title}\n\n{content}";
                // 保存到本地文件
                await File.WriteAllTextAsync(filePath, content);
                i++;

                //Console.WriteLine($"Downloaded {chapter.Title} - {chapter.Url}");
                // 休息 10 秒
                await Task.Delay(opts.Wait * 1000);
            }

            return 0;
        }

        internal static async Task<string> DownloadChapter(string url,string selector,string next, string regex)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(await HttpClientHelper.GetWebHtmlAsync(url));
            var contentHtml = doc.DocumentNode.SelectSingleNode(selector)?.InnerHtml;
            // html 转换为纯文本 p 标签转换为换行

            var content = HtmlEntity.DeEntitize(contentHtml)
            .Replace("<p>", "").Replace("</p>", "\n")
            .Replace("<br>", "\n").Replace("<br/>", "\n").Replace("<br />", "\n").Replace("&nbsp;", " ").Trim();

            // 去除广告
            if (!string.IsNullOrEmpty(regex))
            {
                // 正则匹配的替换
                content = Regex.Replace(content, regex, "");
            }

            // 检查是否有下一页
            var has_next = string.IsNullOrWhiteSpace(next)? null : doc.DocumentNode.SelectSingleNode(next);
            if (has_next != null)
            {
                var nextUrl = has_next.GetAttributeValue("href", "");
                // 处理 nextUrl ，判断 http 、下级 、 / 开头 三种情况，拿到完整的 url
                if(!nextUrl.StartsWith("http")){
                    if (nextUrl.StartsWith("/"))
                    {
                        var uri = new Uri(url);
                        nextUrl = $"{uri.Scheme}://{uri.Host}{nextUrl}";
                    }else{
                        nextUrl = $"{url}/{nextUrl}";
                    }
                }
                var nextContent = await DownloadChapter(nextUrl,selector, next, regex);
                content += nextContent;
            }

            return content;
        }

        internal static async Task<int> MakeBookList(ListOptions opts)
        {
            var url = opts.Url;
            var doc = new HtmlDocument();
            doc.LoadHtml(await HttpClientHelper.GetWebHtmlAsync(url));

            // var doc = new HtmlDocument();
            // doc.Load("list.html");

            var chapterNodes = doc.DocumentNode.SelectNodes(opts.Selector);
            var chapters = chapterNodes.Select(node => new Chapter
            {
                Title = node.InnerText.Trim(),
                Url = opts.Host + node.GetAttributeValue("href", "")
            }).ToList();

            // 存储到 json 文件 使用 Sysytem.Text.Json ,不要转义中文

            var options = new JsonSerializerOptions { Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) };
            var json = JsonSerializer.Serialize(chapters, options);
            File.WriteAllText(opts.Output, json);
            Console.WriteLine($"Found {chapters.Count} chapters, saved to {opts.Output}");
            return 0;
        }

    }

}