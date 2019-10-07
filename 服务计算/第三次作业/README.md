@[TOC](文章目录)
# 1、Selpg的介绍
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
2. 实现：参考C语言版本以及
## 3.6 process_input函数：
1. 任务：进行操作
2. 实现：参考C语言版本以及os/exec 库的用法
