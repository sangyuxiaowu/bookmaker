using System.Text.Json;

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

                content += $"\n{chapter.Title}\n\n{await File.ReadAllTextAsync(file)}";
            }
            await File.WriteAllTextAsync(opts.Output, content);
            return 0;
        }

    }

}