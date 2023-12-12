# BookMaker

# BookMaker 项目

BookMaker 是一个命令行工具，用于从指定 URL 下载书籍，生成目录文件，下载章节，合并章节并生成单个文档，以及将 txt 文件转换为 epub 文件。

## 使用

BookMaker 提供了以下命令：

### `list`

获取指定URL书籍的目录列表，并生成目录文件。

```bash
BookMaker list -u <url> -s <selector> [-h <host>] [-o <output>]
```

参数说明：

- `-u, --url <url>`：设置书籍URL。（必需）
- `-s, --selector <selector>`：设置书籍目录选择器。（必需）
- `-h, --host <host>`：设置章节前面拼接的URL，默认为空。
- `-o, --output <output>`：设置保存文件名，默认为 `chapters.json`。

### `down`

根据目录信息下载章节。

```bash
BookMaker down -j <json> -s <selector> [-d <dir>] [-r <regex>] [-n <next>] [-w <wait>] [-b <begin>]
```

参数说明：

- `-j, --json <json>`：设置书籍目录 json 文件，默认为 `chapters.json`。
- `-s, --selector <selector>`：设置章节内容选择器。（必需）
- `-d, --dir <dir>`：设置书籍保存目录，默认为 `download`。
- `-r, --regex <regex>`：设置广告内容删除正则，默认为空。
- `-n, --next <next>`：设置下一页判断选择器，默认为空。
- `-w, --wait <wait>`：设置等待秒数，默认为 10。
- `-b, --begin <begin>`：设置开始章节，默认为 1。

### `make`

合并章节生成单个文档。

```bash
BookMaker make -j <json> [-d <dir>] [-f <format>] [-o <output>] [-t <title>] [-a <author>] [-i <intro>] [-c <cover>]
```

参数说明：

- `-j, --json <json>`：设置书籍目录 json 文件，默认为 `chapters.json`。
- `-d, --dir <dir>`：设置书籍章节目录，默认为 `download`。
- `-f, --format <format>`：设置书籍类型，默认为 `txt`。可选值为 `txt` 和 `epub`。
- `-o, --output <output>`：设置书籍保存文件名，默认为 `book.txt`。
- `-t, --title <title>`：设置书籍标题，默认为 `book`。
- `-a, --author <author>`：设置书籍作者，默认为空。
- `-i, --intro <intro>`：设置书籍简介，默认为空。
- `-c, --cover <cover>`：设置书籍封面，默认为空。

### `txt2epub`

将 txt 文件转换为 epub 文件。

```bash
BookMaker txt2epub -f <file> [-r <regex>] [-d <debug>] [-o <output>] [-t <title>] [-a <author>] [-i <intro>] [-c <cover>]
```

参数说明：

- `-f, --file <file>`：设置要转换的 txt 文件。（必需）
- `-r, --regex <regex>`：设置目录提取正则，默认为空，使用内置正则。
- `-d, --debug <debug>`：设置是否为测试模式，默认为 `false`。
- `-o, --output <output>`：设置书籍保存文件名，默认为 `book.epub`。
- `-t, --title <title>`：设置书籍标题，默认为 `book`。
- `-a, --author <author>`：设置书籍作者，默认为空。
- `-i, --intro <intro>`：设置书籍简介，默认为空。
- `-c, --cover <cover>`：设置书籍封面，默认为空。
