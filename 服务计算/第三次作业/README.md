# 1、Selpg的介绍
selpg 允许用户指定从输入文本抽取的页的范围，这些输入文本可以来自文件/标准输入/另一个进程。
# 2、具体要求
实现指令的要求请查看该[链接](https://www.ibm.com/developerworks/cn/linux/shell/clutil/index.html)
# 3、实现具体思路
查看selpg 的 [C 代码](https://www.ibm.com/developerworks/cn/linux/shell/clutil/selpg.c)，在此基础上实现。
## 3.1引入的包

```go
import (
	"bufio"
	"flag"
	"fmt"
	"io"
	"os"
	"os/exec"
)
```

os，flag包，简单处理参数；bufio处理缓存
## 3.2相关结构体
按照C语言设置对应结构体和全局变量：

```go
type selpgargs struct {
	start_page  int         //开始的页编号
	end_page    int         //结束的页编号
	in_filename  string
	print_dest string
	page_len    int     /* default value, can be overriden by "-l number" on command line */
	page_type bool  /* 'l' for lines-delimited, 'f' for form-feed-delimited */
}

var progname string     /* program name, for error messages */
```

## 3.3主函数处理流程
初始化参数结构体-处理参数-根据参数进行处理
```go
func main() {
	progname = os.Args[0]    /* save name by which program is invoked, for error messages */
	var args selpgargs  
	FlagInit(&args)         //初始化参数标记
	process_args(&args)      //处理参数
	process_input(&args)     //根据用户输入的各个参数进行相应的操作
}
```

## 3.4FlagInit函数：
1. 任务：进行指令结构体的标记的初始化
2. 实现：参考C语言版本的实现及[flag包的用法](http://blog.studygolang.com/2013/02/%E6%A0%87%E5%87%86%E5%BA%93-%E5%91%BD%E4%BB%A4%E8%A1%8C%E5%8F%82%E6%95%B0%E8%A7%A3%E6%9E%90flag/)
3. flag的简单使用：
A.定义flags有两种方式：

1）flag.Xxx()，其中Xxx可以是Int、String等；返回一个相应类型的指针，如：

```go
var ip = flag.Int("flagname", 1234, "help message for flagname")
```

2）flag.XxxVar()，将flag绑定到一个变量上，如：

```go
var flagvar int
flag.IntVar(&flagvar, "flagname", 1234, "help message for flagname")
```

另外，还可以创建自定义flag，只要实现flag.Value接口即可（要求receiver是指针），这时候可以通过如下方式定义该flag：

```go
flag.Var(&flagVal, "name", "help message for flagname
```
B. 解析参数（Parse）:从参数列表中解析定义的flag。参数arguments不包括命令名，即应该是os.Args[1:]。
4. 代码：

```go
//initial flags
func FlagInit(args *selpgargs) {
	flag.IntVar(&args.start_page, "s", -1, "Start page.")
	flag.IntVar(&args.end_page, "e", -1, "End page.")
	flag.IntVar(&args.page_len, "l", 72, "page_len.")
	flag.BoolVar(&args.page_type, "f", false, "page_type")
	flag.StringVar(&args.print_dest， "d", "", "print_dest")
	flag.Usage = usage
	flag.Parse()
}
```

## 3.5 process_args函数：
1. 任务：处理参数
2. 实现：参考C语言版本
3. 代码：

```go
func process_args(args *selpgargs) {
	if args == nil {
        fmt.Fprintf(os.Stderr, "\n[Error]The args is nil!Please check your program!\n\n")
        os.Exit(1)
    }
    if args.start_page == -1 || args.end_page == -1 {
		fmt.Fprintf(os.Stderr, "\n[Error]:%s, not enough arguments\n\n", progname)
		flag.Usage()
		os.Exit(2)
	}

	if os.Args[1][0] != '-' || os.Args[1][1] != 's' {
		fmt.Fprintf(os.Stderr, "\n[Error]:%s, 2st arg should be -sstart_page\n\n", progname)
		flag.Usage()
		os.Exit(3)
	}

	end_index := 2
	if len(os.Args[1]) == 2 {
		end_index = 3
	}

	if os.Args[end_index][0] != '-' || os.Args[end_index][1] != 'e' {
		fmt.Fprintf(os.Stderr, "\n[Error]:%s, 3st arg should be -eend_page\n\n", progname)
		flag.Usage()
		os.Exit(4)
	}

	if args.start_page > args.end_page || args.start_page < 0 ||
		args.end_page < 0 {
		fmt.Fprintln(os.Stderr, "\n[Error]:Invalid arguments")
		flag.Usage()
		os.Exit(5)
	}

}
```

## 3.6 process_input函数：
1. 任务：进行操作
2. 实现：参考C语言版本以及os/exec 库,bufio包的用法
3. 代码：

```go
func process_input(args *selpgargs) {
	var stdin io.WriteCloser
	var err error
	var cmd *exec.Cmd

	if args.print_dest != "" {
		cmd = exec.Command("cat", "-n")
		stdin, err = cmd.StdinPipe()
		if err != nil {
			fmt.Println(err)
		}
	} else {
		stdin = nil
	}

	if flag.NArg() > 0 {
		args.in_filename = flag.Arg(0)
		output, err := os.Open(args.in_filename)
		if err != nil {
			fmt.Println(err)
			os.Exit(1)
		}
		reader := bufio.NewReader(output)
		if args.page_type {
			for pageNum := 0; pageNum <= args.end_page; pageNum++ {
				line, err := reader.ReadString('\f')
				if err != io.EOF && err != nil {
					fmt.Println(err)
					os.Exit(1)
				}
				if err == io.EOF {
					break
				}
				printOrWrite(args, string(line), stdin)
			}
		} else {
			count := 0
			for {
				line, _, err := reader.ReadLine()
				if err != io.EOF && err != nil {
					fmt.Println(err)
					os.Exit(1)
				}
				if err == io.EOF {
					break
				}
				if count/args.page_len >= args.start_page {
					if count/args.page_len > args.end_page {
						break
					} else {
						printOrWrite(args, string(line), stdin)
					}
				}
				count++
			}

		}
	} else {
		scanner := bufio.NewScanner(os.Stdin)
		count := 0
		target := ""
		for scanner.Scan() {
			line := scanner.Text()
			line += "\n"
			if count/args.page_len >= args.start_page {
				if count/args.page_len <= args.end_page {
					target += line
				}
			}
			count++
		}
		printOrWrite(args, string(target), stdin)
	}

	if args.print_dest != "" {
		stdin.Close()
		cmd.Stdout = os.Stdout
		cmd.Run()
	}
}
```

## 3.7 测试代码
1.  简单测试：
![在这里插入图片描述](https://img-blog.csdnimg.cn/20191007144947695.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L2h1YW5nc2htMjM=,size_16,color_FFFFFF,t_70)
2. 重定向测试
![在这里插入图片描述](https://img-blog.csdnimg.cn/20191007144917786.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L2h1YW5nc2htMjM=,size_16,color_FFFFFF,t_70)
![在这里插入图片描述](https://img-blog.csdnimg.cn/20191007145132343.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L2h1YW5nc2htMjM=,size_16,color_FFFFFF,t_70)
3. 输出到2.txt：
![在这里插入图片描述](https://img-blog.csdnimg.cn/20191007145219577.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L2h1YW5nc2htMjM=,size_16,color_FFFFFF,t_70)
4. 错误提示：
![在这里插入图片描述](https://img-blog.csdnimg.cn/20191007145241905.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L2h1YW5nc2htMjM=,size_16,color_FFFFFF,t_70)
5. 默认行数：
![在这里插入图片描述](https://img-blog.csdnimg.cn/20191007145304107.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L2h1YW5nc2htMjM=,size_16,color_FFFFFF,t_70)
6. 管道输送至命令“lp -dlp1”：
![在这里插入图片描述](https://img-blog.csdnimg.cn/20191007145317541.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L2h1YW5nc2htMjM=,size_16,color_FFFFFF,t_70)
