
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using HtmlAgilityPack;

namespace book
{
    class Program
    {
        static void Main(string[] args)
        {
            // 根据参数执行不同的方法
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: book <command>");
                Console.WriteLine("Commands:");
                Console.WriteLine("  list");
                Console.WriteLine("  download");
                Console.WriteLine("  txt");
                return;
            }

            var command = args[0];
            switch (command)
            {
                case "list":
                    MakeBookList();
                    break;
                case "download":
                    MakeChapter().GetAwaiter().GetResult();
                    break;
                case "txt":
                    MakeTxt();
                    break;
                default:
                    Console.WriteLine($"Unknown command {command}");
                    break;
            }

        }

        static void MakeTxt(){
            // 将 download 目录下的 txt 文件合并为1个
            var files = Directory.GetFiles("download","*.txt");
            var content = "";
            foreach(var file in files){
                content += File.ReadAllText(file);
            }
            File.WriteAllText("book.txt",content);
        }


        static async Task MakeChapter()
        {
            // 创建目录
            Directory.CreateDirectory("download");

            // 判断 json 是否存在
            if (!File.Exists("chapters.json"))
            {
                Console.WriteLine("chapters.json not found, please run `book list` first");
                return;
            }

            var host = "https://www.bxwx5.cc";

            // 读取 chapters.json
            var json = File.ReadAllText("chapters.json");
            var chapters = JsonSerializer.Deserialize<List<Chapter>>(json);

            // 遍历 chapters
            int i = 1;
            foreach (var chapter in chapters)
            {
                Console.WriteLine($"Downloading {i}/{chapters.Count} {chapter.Title} - {chapter.Url}");
                // 判断文件是否存在
                var filePath = $"download/chapter_{i}.txt";
                if (File.Exists(filePath))
                {
                    Console.WriteLine($"File {filePath} already exists, skipping...");
                    i++;
                    continue;
                }
                // 下载章节
                var content = await DownloadChapter(host,chapter.Url,new string[]{"记住回到古代当太子永久地址https://www.bxwx5.cc/bxwx585620.html"});

                content = $"\n{chapter.Title}\n\n{content}";
                // 保存到本地文件
                await File.WriteAllTextAsync(filePath, content);
                i++;

                //Console.WriteLine($"Downloaded {chapter.Title} - {chapter.Url}");
                // 休息 10 秒
                await Task.Delay(10000);
            }

        }

        static async Task<string> DownloadChapter(string host,string url,string[] guanggao = null){
            var web = new HtmlWeb();
            var doc = web.Load(host+url);
            var contentHtml = doc.DocumentNode.SelectSingleNode("//div[@id='chaptercontent']")?.InnerHtml;
            // html 转换为纯文本 p 标签转换为换行
            
            var content = HtmlEntity.DeEntitize(contentHtml)
            .Replace("<p>","").Replace("</p>","\n")
            .Replace("<br>","\n").Replace("<br/>","\n").Replace("<br />","\n").Replace("&nbsp;"," ").Trim();

            // 去除广告
            if(guanggao != null){
                foreach(var item in guanggao){
                    content = content.Replace(item,"");
                }
            }

            // 检查是否有下一页
            var next = doc.DocumentNode.SelectSingleNode("//div[@class='operate']//a[text()='下一页']");
            if(next != null){
                var nextUrl = next.GetAttributeValue("href","");
                var nextContent = await DownloadChapter(host,nextUrl,guanggao);
                content += nextContent;
            }

            return content;
        }

        static void MakeBookList()
        {
            var url = "https://www.bxwx5.cc/bxwx585620.html";
            var web = new HtmlWeb();
            var doc = web.Load(url);

            // var doc = new HtmlDocument();
            // doc.Load("list.html");

            var chapterNodes = doc.DocumentNode.SelectNodes("//div[@class='card mt20 fulldir']//ul//a");
            var chapters = chapterNodes.Select(node => new Chapter
            {
                Title = node.InnerText.Trim(),
                Url = node.GetAttributeValue("href", "")
            }).ToList();

            // 存储到 json 文件 使用 Sysytem.Text.Json ,不要转义中文


            var options = new JsonSerializerOptions { Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) };
            var json = JsonSerializer.Serialize(chapters, options);
            File.WriteAllText("chapters.json", json);

            Console.WriteLine($"Found {chapters.Count} chapters:");
            foreach (var chapter in chapters)
            {
                Console.WriteLine($"{chapter.Title} - {chapter.Url}");
            }

        }
    }
}

