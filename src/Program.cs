using CommandLine;
using BookMaker.BookTool;


namespace BookMaker
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            // 根据参数执行不同的方法， 使用 CommandLineParser 实现参数传递
            return await Parser.Default.ParseArguments<ListOptions, DownloadOptions, MakeOptions, Txt2EpubOptions>(args)
            .MapResult(
                async(ListOptions opts) => await BookScraper.MakeBookList(opts),
                async(DownloadOptions opts) => await BookScraper.MakeChapter(opts),
                async(MakeOptions opts) => await BookBuild.MakeBook(opts),
                errs => Task.FromResult(1));
        }
    }
}

