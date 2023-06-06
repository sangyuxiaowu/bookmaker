using CommandLine;

namespace book
{
    /// <summary>
    /// 获取指定URL书籍的目录列表，并生成目录文件
    /// </summary>
    [Verb("list", HelpText = "获取书籍列表")]
    class ListOptions {
        /// <summary>
        /// 配置ASSK和证书信息
        /// </summary>
        [Option('u', "url", Required = true, HelpText = "设置书籍URL")]
        public string? Url { get; set; }

        /// <summary>
        /// 籍目录选择器
        /// </summary>
        [Option('s', "selector", Required = true, HelpText = "设置书籍目录选择器")]
        public string? Selector { get; set; }

        /// <summary>
        /// 拼接URL，用于获取章节内容
        /// </summary>
        [Option('h', "host", Default ="", HelpText = "设置章节前面拼接的URL，默认为空")]
        public string? Host { get; set; }

        /// <summary>
        /// 保存文件名
        /// </summary>
        [Option('o', "output", Default ="chapters.json", HelpText = "设置保存文件名，默认为 chapters.json")]
        public string? Output { get; set; }
    }

    /// <summary>
    /// 下载章节文件
    /// </summary>
    [Verb("down", HelpText = "根据目录信息下载章节")]
    class DownloadOptions{
        /// <summary>
        /// 书籍目录 json 文件
        /// </summary>
        [Option('j', "json", Default ="chapters.json", HelpText = "设置书籍目录 json 文件，默认为 chapters.json")]
        public string? Json { get; set; }

        /// <summary>
        /// 书籍保存目录
        /// </summary>
        [Option('d', "dir", Default ="download", HelpText = "设置书籍保存目录，默认为 download")]
        public string? Dir { get; set; }

        /// <summary>
        /// 章节内容选择器
        /// </summary>
        [Option('s', "selector", Required = true, HelpText = "设置章节内容选择器")]
        public string? Selector { get; set; }

        /// <summary>
        /// 广告内容删除正则 数组
        /// </summary>
        [Option('r', "regex", Default ="", HelpText = "设置广告内容删除正则，默认为空")]
        public string? Regex { get; set; }

        /// <summary>
        /// 下一页判断选择器
        /// </summary>
        [Option('n', "next", Default ="", HelpText = "设置下一页判断选择器，默认为空")]
        public string? Next { get; set; }

        /// <summary>
        /// 等待秒数
        /// </summary>
        [Option('w', "wait", Default =10, HelpText = "设置等待秒数，默认为 10")]
        public int Wait { get; set; }

    }

    /// <summary>
    /// 合并章节文件
    /// </summary>
    [Verb("make", HelpText = "合并章节生成单个文档")]
    class MakeOptions{
        /// <summary>
        /// 书籍章节目录
        /// </summary>
        [Option('d', "dir", Default ="download", HelpText = "设置书籍章节目录，默认为 download")]
        public string? Dir { get; set; }

        /// <summary>
        /// 书籍保存文件名
        /// </summary>
        [Option('o', "output", Default ="book.txt", HelpText = "设置书籍保存文件名，默认为 book.txt")]
        public string? Output { get; set; }

    }




}