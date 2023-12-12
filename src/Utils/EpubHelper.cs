
using NovelEpubMaker;

namespace BookMaker.Utils
{
    public static class EpubHelper
    {
        public static async Task<byte[]> MakeEpub(string Title, List<NovelContent> chapters, string Author = "佚名",string Intro = "无", string Cover = ""){
            // 创建 epub 文件
            var epub = new NovelEpub{
                Metadata = new EpubMetadata{
                    Title = Title,
                    Author = Author,
                    Description = Intro,
                },
                NovelList = chapters,
                CoverBase64 = string.IsNullOrEmpty(Cover)? "":
                    Convert.ToBase64String(await File.ReadAllBytesAsync(Cover)),
            };
            return await epub.SaveBytesAsync();
        }

    }
}