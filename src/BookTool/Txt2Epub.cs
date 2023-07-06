using System.Text.RegularExpressions;
using BookMaker.Utils;
using NovelEpubMaker;

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
            if(chapters.Count == 0){
                Console.WriteLine($"No chapters found in {opts.Txt}, exiting...");
                return 1;
            }
            // 生成 epub 文件
            var epub = new NovelEpub{
                Metadata = new EpubMetadata{
                    Title = opts.Title,
                    Author = opts.Author,
                    Description = opts.Intro,
                },
                NovelList = chapters,
                CoverBase64 = string.IsNullOrEmpty(opts.Cover)? "":
                    Convert.ToBase64String(await File.ReadAllBytesAsync(opts.Cover)),
            };
            // 保存 epub 文件
            await File.WriteAllBytesAsync(opts.Output, await epub.SaveBytesAsync());
            return 0;
        }


        /// <summary>
        /// 章节提取
        /// </summary>
        /// <param name="txt">文本内容</param>
        /// <returns>章节信息</returns>
        static List<NovelContent> GetChapters(string txt){
            var chapters = new List<NovelContent>();
            var pattern = @"\s*第\s*[0-9一二三四五六七八九十零〇百千万两]+\s*[部章节回]\s*.*\s*";
            var matches = Regex.Matches(txt, pattern);
            if(matches.Count < 2){
                return chapters;
            }
            // 处理正文开始前有部分内容的情况
            if(matches[0].Index > 0){
                var chapter = new NovelContent{
                    Title = "前言",
                    Content = txt[..matches[0].Index]
                };
                if(!string.IsNullOrWhiteSpace(chapter.Content)){
                    chapters.Add(chapter);
                }
            }
            // 处理章节内容
            for(var i = 0; i < matches.Count; i++){
                var chapter = new NovelContent{
                    Title = matches[i].Value.Trim(),
                    Content = (i==matches.Count-1) ? txt[matches[i].Index..] : txt[matches[i].Index..matches[i + 1].Index]
                };
                chapters.Add(chapter);
            }
            return chapters;
        }
    }
}