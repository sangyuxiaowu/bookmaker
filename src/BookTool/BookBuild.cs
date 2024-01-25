using System.Text.Json;
using BookMaker.Utils;
using NovelEpubMaker;

namespace BookMaker.BookTool{

    internal static class BookBuild{

        internal static async Task<int> MakeBook(MakeOptions opts){
            // 判断 json 是否存在
            if (!File.Exists(opts.Json))
            {
                Console.WriteLine($"File {opts.Json} not exists, exiting...");
                return 1;
            }

            // 检查输出文件是否存在，存在则删除
            if (File.Exists(opts.Output))
            {
                File.Delete(opts.Output);
            }

            return opts.Format switch
            {
                BookType.Txt => await MakeTxt(opts),
                BookType.Epub => await MakeEpub(opts),
                _ => MakeErr(opts),
            };
        }

        static int MakeErr(MakeOptions opts){
            Console.WriteLine($"Format {opts.Format} not supported");
            return 1;
        }

        static async Task<int> MakeTxt(MakeOptions opts){
            var json = await File.ReadAllTextAsync(opts.Json);
            var chapters = JsonSerializer.Deserialize<List<Chapter>>(json);

            // 将 download 目录下的 txt 文件合并为1个
            var files = Directory.GetFiles(opts.Dir, "*.txt");
            var content = "";
            foreach (var file in files)
            {
                // 通过文件名获取章节序号
                var fileName = Path.GetFileNameWithoutExtension(file);
                var index = int.Parse(fileName.Split("_")[1]);
                var chapter = chapters[index - 1];
                if (opts.Number)
                {
                    content += $"\n第{index}章 {chapter.Title}\n\n{await File.ReadAllTextAsync(file)}";
                }
                else
                {
                    content += $"\n{chapter.Title}\n\n{await File.ReadAllTextAsync(file)}";
                }
            }
            await File.WriteAllTextAsync(opts.Output, content);
            return 0;
        }

        static async Task<int> MakeEpub(MakeOptions opts){
            var json = await File.ReadAllTextAsync(opts.Json);
            var chapters = JsonSerializer.Deserialize<List<Chapter>>(json);

            var files = Directory.GetFiles(opts.Dir, "*.txt");
            List<NovelContent> novellist = new List<NovelContent>();
            foreach (var file in files)
            {
                // 通过文件名获取章节序号
                var fileName = Path.GetFileNameWithoutExtension(file);
                var index = int.Parse(fileName.Split("_")[1]);
                var chapter = chapters[index - 1];

                novellist.Add(new NovelContent{
                    Title = opts.Number ? $"第{index}章 {chapter.Title}" : chapter.Title,
                    Content = await File.ReadAllTextAsync(file),
                });
            }

            // 创建 epub 文件
            var btyes = await EpubHelper.MakeEpub(opts.Title,novellist,opts.Author,opts.Intro,opts.Cover);
            await File.WriteAllBytesAsync(opts.Output, btyes);
            return 0;
        }

    }

}