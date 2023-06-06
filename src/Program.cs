
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using HtmlAgilityPack;
using CommandLine;
using System.Text.RegularExpressions;

namespace book
{
    class Program
    {
        static int Main(string[] args)
        {
            // 根据参数执行不同的方法， 使用 CommandLineParser 实现参数传递

            return CommandLine.Parser.Default.ParseArguments<ListOptions, DownloadOptions, MakeOptions>(args)
            .MapResult(
                (ListOptions opts) => MakeBookList(opts),
                (DownloadOptions opts) => MakeChapter(opts).GetAwaiter().GetResult(),
                (MakeOptions opts) => MakeTxt(opts),
                errs => 1);

            //MakeTxt();

        }

        static int MakeTxt(MakeOptions opts)
        {
            // 将 download 目录下的 txt 文件合并为1个
            var files = Directory.GetFiles(opts.Dir, "*.txt");
            var content = "";
            foreach (var file in files)
            {
                content += File.ReadAllText(file);
            }
            File.WriteAllText(opts.Output, content);
            return 0;
        }


        static async Task<int> MakeChapter(DownloadOptions opts)
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

            // 遍历 chapters
            int i = 1;
            foreach (var chapter in chapters)
            {
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

                content = $"\n{chapter.Title}\n\n{content}";
                // 保存到本地文件
                await File.WriteAllTextAsync(filePath, content);
                i++;

                //Console.WriteLine($"Downloaded {chapter.Title} - {chapter.Url}");
                // 休息 10 秒
                await Task.Delay(opts.Wait * 1000);
            }

            return 0;
        }

        static async Task<string> DownloadChapter(string url,string selector,string next, string regex)
        {
            var web = new HtmlWeb();
            var doc = web.Load(url);
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
            var has_next = doc.DocumentNode.SelectSingleNode(next);
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

        static int MakeBookList(ListOptions opts)
        {
            var url = opts.Url;
            var web = new HtmlWeb();
            var doc = web.Load(url);

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

