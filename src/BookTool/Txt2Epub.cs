using System.Text.RegularExpressions;

namespace BookMaker.BookTool
{
    internal static class Txt2Epub{
        internal static async Task<int> MakeEpub(Txt2EpubOptions opts){
            // 检查 TXT 文件是否存在
            if (!File.Exists(opts.Txt))
            {
                Console.WriteLine($"File {opts.Txt} not exists, exiting...");
                return 1;
            }
            // 对 TXT 文件进行章节提取
            var txt = await File.ReadAllTextAsync(opts.Txt);
            var chapters = GetChapters(txt);
   
            return 0;
        }


        /// <summary>
        /// 章节提取
        /// </summary>
        /// <param name="txt">文本内容</param>
        /// <returns>章节信息</returns>
        static List<Chapter> GetChapters(string txt){
            var chapters = new List<Chapter>();
            var pattern = @"\s*第\s*[0-9一二三四五六七八九十零〇百千万两]+\s*[部章节回]\s*.*\s*";
            var matches = Regex.Matches(txt, pattern);
            for(var i = 0; i < matches.Count; i++){
                var chapter = new Chapter{
                    Title = matches[i].Value.Trim(),
                    Content = (i==matches.Count-1) ? txt[matches[i].Index..] : txt[matches[i].Index..matches[i + 1].Index]
                };
                chapters.Add(chapter);
            }
            return chapters;
        }
    }
}